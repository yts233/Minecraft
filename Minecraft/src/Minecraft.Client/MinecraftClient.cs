using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Minecraft.Extensions;
using Minecraft.Protocol;
using Minecraft.Protocol.Packets;
using Minecraft.Protocol.Packets.Client;
using Minecraft.Protocol.Packets.Server;

namespace Minecraft.Client
{
    public class MinecraftClient
    {
        private static Logger<MinecraftClient> _logger = Logger.GetLogger<MinecraftClient>();
        internal string _playerName;
        internal int _protocolVersion = 756;
        internal bool _offlineMode = true;
        //private MinecraftClientAdapter _adapter;

        /// <summary>
        /// 客户端状态
        /// </summary>
        public MinecraftClientState State { get; private set; } = MinecraftClientState.InTitle;

        public string Server { get; private set; }
        public bool IsConnected => false;//_adapter.IsConnected;

        public MinecraftClient(string playerName)
        {
            _playerName = playerName;
            _logger.Info($"login client: {playerName}");
        }

        /// <summary>
        /// 强制切换协议版本号
        /// </summary>
        /// <param name="version">协议版本号</param>
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
                throw new InvalidOperationException($"invalid client state {State}, it should be InTitle.");
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
                _ = Task.Run(async () => // timeout
                {
                    await Task.Delay(timeout);
                    if (adapter.Running)
                    {
                        timedOut = true;
                        adapter.Stop();
                        client.Close();
                        _logger.Warn($"{hostname}:{port} timed out");
                    }
                });
                adapter.Start(); // start adapter
                adapter.SendPacket(new HandshakePacket
                {
                    NextState = ProtocolState.Status,
                    ProtocolVersion = _protocolVersion,
                    ServerAddress = hostname,
                    ServerPort = port
                });
                adapter.SendPacket(new StatusRequestPacket());
                var statusResponsePacket = (StatusResponsePacket)adapter.ReceivePacket();
                handshaked = true;
                result.LoadContent(statusResponsePacket.Content);
                time1 = DateTime.Now;
                adapter.SendPacket(new StatusPingPacket { Payload = time1.ToUnixTimeStamp() });
                _ = (StatusPongPacket)adapter.ReceivePacket();
                result.Delay = (int)(DateTime.Now - time1).TotalMinutes;
            }
            catch
            {
                if (!handshaked)
                    throw;
            }
            finally
            {
                adapter?.Stop();
                client?.Close();
                client?.Dispose();
            }
            if (timedOut)
                _logger.Warn("Pinging timeout");
            return result;
        }
#if false
        public async Task Connect(string hostname, ushort port)
        {
            if (State == MinecraftClientState.InGame)
                throw new InvalidOperationException("You have already connected to the server.");
            _logger.Info($"Connecting {hostname}, {port}");
            _adapter = new MinecraftClientAdapter(hostname, port, this);
            await _adapter.Connect();
        }

        public async Task Disconnect()
        {
            await _adapter.Disconnect();
        }

        public async Task Chat(string message)
        {
            await _adapter.SendChatPacket(message);
        }
#endif
    }
}