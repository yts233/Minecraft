using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Minecraft;
using Minecraft.Extensions;
using Minecraft.Protocol.MCVersions.MC1171;
using Minecraft.Protocol.MCVersions.MC1171.Packets.Client;
using Minecraft.Protocol.Packets;
using OpenTK.Mathematics;

Console.WriteLine("Hello World!");

var box = new Box2d(-.3D, 0D, .3D, 1.75D);
Console.WriteLine(box.Translated((1D, 1D)));

box.Max += (1, 1);
box.Min += (1, 1);
Console.WriteLine(box);

return;

MC1171Client client = new("MCConsoleTest");

client.Disconnected += (_, e) =>
{
    Logger.WaitForLogging();
    Console.WriteLine($"\n\nDisconnected. Reason: {e}\n\n\n\n\n\n\n\n\n\n\n");
    Println("Reconnecting in 5 seconds...", ConsoleColor.Yellow);
    Task.Run(async () =>
    {
        await Task.Delay(5000);
        if (!client.IsConnected)
            client.Reconnect();
    });
};

client.Connect("localhost", 25566);

while (Console.ReadKey().Key != ConsoleKey.Escape) ;

/*
static void TestPacket(IPacket packet, bool compressed = false)
{
    var stream = new MemoryStream();
    Console.WriteLine(packet.GetPropertyInfoString());
    var adapter = new MCVer756ProtocolAdapter(stream, packet.BoundTo)
    {
        CompressThreshold = compressed ? 1 : 0
    };
    adapter.Start(receiveThread: false);
    adapter.SendPacket(packet);
    adapter.WaitUntilAllPacketsSent();
    Console.WriteLine($"Position/Length: {stream.Position}/{stream.Length}");
    adapter = new MCVer756ProtocolAdapter(stream, adapter.RemoteBoundTo)
    {
        State = packet.State,
        CompressThreshold = compressed ? 1 : 0
    };
    stream.Position = 0;
    adapter.Start(sendThread: false);
    packet = adapter.ReceivePacket();
    Console.WriteLine(packet.GetPropertyInfoString());
    Console.WriteLine($"Position/Length: {stream.Position}/{stream.Length}");
}
TestPacket(new LoginStartPacket { Name = "aaaaaaaaa啊啊啊啊啊" }, true);

*/

Logger.WaitForLogging();

void Print(object obj, ConsoleColor? color = null)
{
    var s = obj.ToString();
    lock (Console.Out)
    {
        if (color != null)
            Console.ForegroundColor = color.Value;
        Console.Write(s);
        Console.ResetColor();
    }
}

void Println(object obj, ConsoleColor? color = null)
{
    var s = obj.ToString();
    lock (Console.Out)
    {
        if (color != null)
            Console.ForegroundColor = color.Value;
        Console.WriteLine(s);
        Console.ResetColor();
    }
}