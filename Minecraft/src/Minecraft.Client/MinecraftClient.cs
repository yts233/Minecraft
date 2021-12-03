using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Minecraft.Client.Handlers;
using Minecraft.Client.Internal;
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
        public ServerListPingResult ServerListPing(string hostname, ushort port)
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
                client = new TcpClient();
                client.Connect(hostname, port);
                adapter = new ProtocolAdapter(client.GetStream(), PacketBoundTo.Server);
                adapter.Start();
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
                adapter.WritePacket(new HandshakePacket //handshake
                {
                    NextState = ProtocolState.Status,
                    ProtocolVersion = _protocolVersion,
                    ServerAddress = hostname,
                    ServerPort = port
                });
                adapter.WritePacket(new StatusRequestPacket());
                var statusResponsePacket = (StatusResponsePacket)adapter.ReadPacket();
                handshaked = true;
                result.LoadContent(statusResponsePacket.Content);
                time1 = DateTime.Now;
                adapter.WritePacket(new StatusPingPacket { Payload = time1.ToUnixTimeStamp() });
                if ((StatusPongPacket)adapter.ReadPacket() != null)
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

        private string _lastServerHostname;
        private ushort _lastServerPort;

        /// <summary>
        /// 重新连接到服务器
        /// </summary>
        public void Reconnect()
        {
            if (_lastServerHostname == null)
                throw new InvalidOperationException("You should connect first before reconnect.");
            Connect(_lastServerHostname, _lastServerPort);
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public void Connect(string hostname, ushort port)
        {
            _lastServerHostname = hostname;
            _lastServerPort = port;
            if (State == MinecraftClientState.InGame)
                _adapter.Disconnect();
            _logger.Info($"Connecting {hostname}, {port}");
            _adapter = new MinecraftClientAdapter(hostname, port, this);
            _adapter.Disconnected += (sender, e) =>
            {
                State = MinecraftClientState.InTitle;
                Disconnected?.Invoke(sender, e);
                _world = null;
                _player = null;
            };
            _adapter.Chat += (sender, e) =>
            {
                //TODO: Use ChatEventArgs and Minecraft.Text.RichText
                ChatReceived?.Invoke(sender, e.jsonData);
            };
            _player = new ClientPlayerEntityHandler(_adapter);
            _world = new WorldHandler(_adapter, _player);
            _adapter.Connect();
            State = MinecraftClientState.InGame;
        }

        /// <summary>
        /// 断开与服务器的连接
        /// </summary>
        /// <returns></returns>
        public void Disconnect()
        {
            if (State != MinecraftClientState.InGame)
                return;
            _adapter.Disconnect();
            State = MinecraftClientState.InTitle;
        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void SendChatMessage(string message)
        {
            if (!IsJoined)
                throw new InvalidOperationException("You cannot chat until join the server");
            _adapter.SendChatPacket(message);
        }

        #region Events
        public event EventHandler<string> Disconnected;
        public event EventHandler<string/*TODO: Use ChatEventArgs and Minecraft.Text.RichText*/> ChatReceived;
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