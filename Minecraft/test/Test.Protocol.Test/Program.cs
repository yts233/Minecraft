using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Minecraft.Extensions;
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
            static async Task TestPacket(Packet packet, bool compressed = false)
            {
                var stream = new MemoryStream();
                Console.WriteLine(packet.GetPropertyInfoString());
                var adapter = new ProtocolAdapter(stream, packet.BoundTo)
                {
                    Compressing = compressed,
                    Threshold = 1,
                    AutoHandleSpecialPacket = false
                };
                await adapter.WritePacketAsync(packet);
                Console.WriteLine($"Position/Length: {stream.Position}/{stream.Length}");
                adapter = new ProtocolAdapter(stream, adapter.RemoteBoundTo)
                {
                    State = packet.State,
                    Compressing = compressed,
                    Threshold = 1,
                    AutoHandleSpecialPacket = false
                };
                stream.Position = 0;
                packet = await adapter.ReadPacketAsync();
                Console.WriteLine(packet.GetPropertyInfoString());
                Console.WriteLine($"Position/Length: {stream.Position}/{stream.Length}");
            }
            await TestPacket(new KeepAlivePacket { KeepAliveId = 1234567 }, true);
            await TestPacket(new KeepAliveResponsePacket { KeepAliveId = 1234567 }, true);
        }
    }
}