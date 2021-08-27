using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Minecraft.Client.Handlers;
using Minecraft.Extensions;
using Minecraft.Protocol;
using Minecraft.Protocol.Packets;
using Minecraft.Protocol.Packets.Client;
using Minecraft.Protocol.Packets.Server;

namespace Minecraft.Client
{
    public class MinecraftClient
    {
        private static readonly Logger<MinecraftClient> _logger = Logger.GetLogger<MinecraftClient>();
        internal string _playerName;
        internal int _protocolVersion = 756;
        internal bool _offlineMode = true;
        private MinecraftClientAdapter _adapter;
        private IClientPlayerHandler _player;
        private IWorldHandler _world;

        /// <summary>
        /// 客户端状态
        /// </summary>
        public MinecraftClientState State { get; private set; } = MinecraftClientState.InTitle;

        public bool IsConnected => _adapter?.IsConnected ?? false;
        public bool IsJoined => _adapter?.IsJoined ?? false;

        public MinecraftClient(string playerName)
        {
            _playerName = playerName;
            _logger.Info($"login client: {playerName}");
        }

        /// <summary>
        /// 强制切换协议版本号
        /// </summary>
        /// <param name="version">协议版本号</param>
        /// <remarks>字面意思，不会适配相应协议版本的内容</remarks>
        public void SwitchProtocolVersion(int version)
        {
            _protocolVersion = version;
            _logger.Info($"force protocol version to {version}.");
        }

        /// <summary>
        /// 发送服务器列表Ping请求
        /// </summary>
        /// <param name="hostname">主机名</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public async Task<ServerListPingResult> ServerListPing(string hostname, ushort port)
        {
            if (State != MinecraftClientState.InTitle)
                throw new InvalidOperationException($"invalid client state {State}, it should be {MinecraftClientState.InTitle}.");
            const int timeout = 10000;
            var timedOut = false;
            var handshaked = false;
            _logger.Info($"Pinging {hostname}, {port}");
            var result = new ServerListPingResult
            {
                Delay = -1
            };
            DateTime time1;
            TcpClient client = null;
            ProtocolAdapter adapter = null;
            try
            {
                await Task.Yield();
                client = new TcpClient();
                await client.ConnectAsync(hostname, port);
                adapter = new ProtocolAdapter(client.GetStream(), PacketBoundTo.Server);
                _ = Task.Run(async () => // timeout
                {
                    await Task.Delay(timeout);
                    if (adapter.State != ProtocolState.Closed)
                    {
                        timedOut = true;
                        client.Close();
                        _logger.Warn($"{hostname}:{port} timed out");
                    }
                });
                await adapter.WritePacketAsync(new HandshakePacket //handshake
                {
                    NextState = ProtocolState.Status,
                    ProtocolVersion = _protocolVersion,
                    ServerAddress = hostname,
                    ServerPort = port
                });
                await adapter.WritePacketAsync(new StatusRequestPacket());
                var statusResponsePacket = (StatusResponsePacket)await adapter.ReadPacketAsync();
                handshaked = true;
                result.LoadContent(statusResponsePacket.Content);
                time1 = DateTime.Now;
                await adapter.WritePacketAsync(new StatusPingPacket { Payload = time1.ToUnixTimeStamp() });
                if ((StatusPongPacket)await adapter.ReadPacketAsync() != null)
                    result.Delay = (int)(DateTime.Now - time1).TotalMinutes;
                adapter.Close();
            }
            catch
            {
                if (!handshaked)
                    throw;
            }
            finally
            {
                adapter?.Dispose();
                client?.Dispose();
            }
            if (timedOut)
                _logger.Warn("Pinging timeout");
            return result;
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task Connect(string hostname, ushort port)
        {
            if (State == MinecraftClientState.InGame)
                await _adapter.Disconnect();
            _logger.Info($"Connecting {hostname}, {port}");
            _adapter = new MinecraftClientAdapter(hostname, port, this);
            _adapter.Disconnected += (sender, e) =>
            {
                State = MinecraftClientState.InTitle;
                Disconnected?.Invoke(sender, e);
                _world = null;
                _player = null;
            };
            _player = new ClientPlayerEntityHandler(_adapter);
            _world = new WorldHandler(_adapter, _player);
            await _adapter.Connect();
            State = MinecraftClientState.InGame;
        }

        /// <summary>
        /// 断开与服务器的连接
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            if (State != MinecraftClientState.InGame)
                return;
            await _adapter.Disconnect();
            State = MinecraftClientState.InTitle;
        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Chat(string message)
        {
            if (!IsJoined)
                throw new InvalidOperationException("You cannot chat until join the server");
            await _adapter.SendChatPacket(message);
        }

        #region Events
        public event EventHandler<string> Disconnected;
        #endregion

        /// <summary>
        /// 获取<see cref="MinecraftClientAdapter"/>
        /// </summary>
        /// <returns></returns>
        [Obsolete("不建议使用")]
        public MinecraftClientAdapter GetAdapter()
        {
            return IsConnected ? _adapter : null;
        }

        public IWorldHandler GetWorld()
        {
            return IsJoined ? _world : null;
        }

        public IClientPlayerHandler GetPlayer()
        {
            return IsJoined ? _player : null;
        }
    }
}