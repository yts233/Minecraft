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
        private readonly string _playerName;
        private int _protocolVersion = 755;

        public MinecraftClient(string playerName)
        {
            _playerName = playerName;
            Logger.Info<MinecraftClient>($"login client: {playerName}");
        }

        public void SwitchProtocolVersion(int version)
        {
            _protocolVersion = version;
            Logger.Info<MinecraftClient>($"switch protocol version into {version}.");
        }

        public async Task<ServerListPingResult> ServerListPingAsync(string hostname, ushort port)
        {
            var result = new ServerListPingResult();
            long time1 = 0;
            using var client = new TcpClient();
            await client.ConnectAsync(await Dns.GetHostAddressesAsync(hostname), port);
            var adapter = new ProtocolAdapter(client.GetStream(), PacketBoundTo.Server);
            adapter.Started += (_, _) =>
            {
                adapter.SendPacket(new HandshakePacket
                {
                    NextState = ProtocolState.Status,
                    ProtocolVersion = _protocolVersion,
                    ServerAddress = hostname,
                    ServerPort = port
                }).LogException<HandshakePacket>();
            };
            adapter.HandleSendPacket<HandshakePacket>(_ => adapter.SendPacket(new StatusRequestPacket()).LogException<StatusRequestPacket>());
            adapter.HandlePacket<StatusResponsePacket>(packet =>
            {
                result.Content = packet.Content;
                time1 = DateTime.Now.ToUnixTimeStamp();
                adapter.SendPacket(new StatusPingPacket {Payload = time1}).LogException<StatusPingPacket>();
            });
            adapter.HandlePacket<StatusPongPacket>(packet =>
            {
                result.Delay = packet.Payload - time1;
                adapter.Stop();
            });
            await adapter.Run();
            client.Close();
            return result;
        }
    }

    public class ServerListPingResult
    {
        public string Content { get; set; }

        /// <summary>
        /// Delay
        /// </summary>
        /// <remarks>毫秒级</remarks>
        public long Delay { get; set; }
    }
}