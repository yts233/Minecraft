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
        internal string _playerName;
        internal int _protocolVersion = 756;
        internal bool _offlineMode = true;
        private MinecraftClientAdapter _adapter;

        /// <summary>
        /// 客户端状态
        /// </summary>
        public MinecraftClientState State { get; private set; } = MinecraftClientState.InTitle;

        public string Server { get; private set; }

        public MinecraftClient(string playerName)
        {
            _playerName = playerName;
            _ = Logger.Info<MinecraftClient>($"login client: {playerName}");
        }

        /// <summary>
        /// 切换协议版本号
        /// </summary>
        /// <param name="version">协议版本号</param>
        public void SwitchProtocolVersion(int version)
        {
            _protocolVersion = version;
            _ = Logger.Info<MinecraftClient>($"switch protocol version into {version}.");
        }

        public void ChangeOfflineUserName(string name)
        {
            _ = Logger.Info<MinecraftClient>($"Change username: {name}");
            _playerName = name;
        }

        /// <summary>
        /// 发送服务器列表Ping请求
        /// </summary>
        /// <param name="hostname">主机名</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public async Task<ServerListPingResult> ServerListPingAsync(string hostname, ushort port)
        {
            if (State != MinecraftClientState.InTitle)
                throw new InvalidOperationException($"invalid client state {State}, it should be InTitle.");
            const int timeout = 10000;
            var timedOut = false;
            var handshaked = false;
            _ = Logger.Info<MinecraftClient>($"Pinging {hostname}, {port}");
            var result = new ServerListPingResult
            {
                Delay = -1
            };
            DateTime time1;
            using var client = new TcpClient();
            await client.ConnectAsync(hostname, port);
            var adapter = new ProtocolAdapter(client.GetStream(), PacketBoundTo.Server);
            async Task Run()
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(timeout);
                    if (adapter.Running)
                    {
                        timedOut = true;
                        _ = adapter.Stop();
                        client.Close();
                        _ = Logger.Warn<MinecraftClient>($"{hostname}:{port} timed out");
                    }
                });
                await adapter.SendPacket(new HandshakePacket
                {
                    NextState = ProtocolState.Status,
                    ProtocolVersion = _protocolVersion,
                    ServerAddress = hostname,
                    ServerPort = port
                });
                await adapter.SendPacket(new StatusRequestPacket());
                var statusResponsePacket = await adapter.ReceiveSinglePacket<StatusResponsePacket>();
                handshaked = true;
                time1 = DateTime.Now;
                await adapter.SendPacket(new StatusPingPacket { Payload = time1.ToUnixTimeStamp() });
                await adapter.ReceiveSinglePacket<StatusPongPacket>().HandleException();
                result.LoadContent(statusResponsePacket.Content);
                result.Delay = (int)(DateTime.Now - time1).TotalMinutes;
                await adapter.Stop();
            }
            adapter.Started += async (_, _) =>
            {
                await Run().HandleException(ex => _ = adapter.Stop());
            };
            Exception exception = null;
            adapter.Exception += (_, e) =>
            {
                if (!handshaked)
                {
                    exception = timedOut ? new TimeoutException() : e;
                    return;
                }
                if (timedOut) return;
                _ = Logger.Fatal<MinecraftClient>("Protocol error: " + e.Message);
            };
            await adapter.Start();
            client.Close();
            if (exception != null)
            {
                throw exception;
            }
            return result;
        }

        public async Task Connect(string hostname, ushort port)
        {
            if (State == MinecraftClientState.InGame)
                throw new InvalidOperationException("You have already connected to the server.");
            _ = Logger.Info<MinecraftClient>($"Connecting {hostname}, {port}");
            _adapter = new MinecraftClientAdapter(hostname, port, this);
            await _adapter.Connect();
        }
    }
}