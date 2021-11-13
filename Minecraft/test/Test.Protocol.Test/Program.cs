using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Minecraft.Extensions;
using Minecraft.Protocol;
using Minecraft.Protocol.Packets;
using Minecraft.Protocol.Packets.Client;
using Minecraft.Protocol.Packets.Server;
using ClientChatMessagePacket = Minecraft.Protocol.Packets.Client.ChatMessagePacket;
using ServerChatMessagePacket = Minecraft.Protocol.Packets.Server.ChatMessagePacket;
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
            await TestPacket(new ServerChatMessagePacket { JsonData = "aaaaaaaaa啊啊啊啊啊" }, true);
            await TestPacket(new ClientChatMessagePacket { Message = "aaaaaaaaa啊啊啊啊啊" }, true);
        }
    }
}