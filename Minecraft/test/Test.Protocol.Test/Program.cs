using System;
using System.Net;
using System.Threading.Tasks;
using Minecraft;
using Minecraft.Protocol;
using Minecraft.Protocol.Packets;
using Minecraft.Protocol.Packets.Client;
using Minecraft.Protocol.Packets.Server;

namespace Test.Protocol.Test
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var adapter =
                new ProtocolAdapter(
                    new IPEndPoint( /*await Dns.GetHostAddressesAsync("localhost")).First()*/
                        IPAddress.Parse("222.187.239.233"), 25565),
                    PacketOrigin.Server);
            var adapterTask = adapter.Run();
            adapter.PacketSent += (sender, e) => Logger.Info<Program>($"packet sent: {e.GetType().Name}");
            adapter.PacketReceived += (sender, e) => Logger.Info<Program>($"packet received: {e.GetType().Name}");
            var now = (DateTime.Now - DateTime.UnixEpoch).Ticks;
            adapter.HandlePacket(delegate(StateResponsePacket packet) { Console.WriteLine(packet.Content); });
            adapter.HandlePacket(delegate(StatePongPacket packet)
            {
                Logger.Info<Program>(
                    $"pong packet received, ping: {(int) new TimeSpan(packet.Payload - now).TotalMilliseconds}ms");
            });
            await adapter
                .SendPacket(new HandshakePacket
                {
                    ProtocolVersion = 340,
                    ServerAddress = "222.187.239.233",
                    ServerPort = 25565,
                    NextState = ProtocolState.Status
                })
                .SendPacket(new StateRequestPacket())
                .SendPacket(new StatePingPacket {Payload = now})
                .FlushAsync();
            await adapterTask;
        }
    }
}