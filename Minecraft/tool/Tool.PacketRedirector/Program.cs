using Minecraft;
using Minecraft.Extensions;
using Minecraft.Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Minecraft.Protocol.Packets;

namespace Tool.PacketRedirector
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Logger.SetExceptionHandler();
            await Logger.HelloWorld<Program>("PacketRedirector");
            var tcpListener = new TcpListener(IPAddress.Any, 25565);
            const string serverAddress = "localhost";
            const ushort serverPort = 25566;
            while (true)
            {
                TcpClient server = null, client = null;
                try
                {
                    tcpListener.Start();
                    client = await tcpListener.AcceptTcpClientAsync();
                    server = new TcpClient();
                    server.Connect(serverAddress, serverPort);
                    var clientAdapter = new ProtocolAdapter(client.GetStream(), PacketBoundTo.Client);
                    var serverAdapter = new ProtocolAdapter(server.GetStream(), PacketBoundTo.Server);
                    serverAdapter.PacketReceived += async (_, packet) =>
                    {
                        await clientAdapter.SendPacket(packet).LogException<Program>();
                        //await Logger.Info<Program>($"[{packet.State}] S -> C {packet.GetPropertyInfoString()}");
                        await Logger.Info<Program>($"[{packet.State}] S -> C 0x{packet.PacketId.ToString("X").PadLeft(2, '0')}");
                    };
                    serverAdapter.UnregisteredPacketReceived += async (_, e) =>
                    {
                        var dataPacket = e.dataPacket;
                        await clientAdapter.SendDataPacket(dataPacket).LogException<Program>();
                        var content = dataPacket.Content;
                        content.Position = 0;
                        var printer = new StreamPrinter(content);
                        await Logger.Info<Program>($"[{dataPacket.State}] S -> C: Unknow packet: 0x{dataPacket.PacketId.ToString("X").PadLeft(2, '0')}");
                        //printer.Print();
                    };
                    clientAdapter.PacketReceived += async (_, packet) =>
                    {
                        await serverAdapter.SendPacket(packet).LogException<Program>();
                        //await Logger.Info<Program>($"[{packet.State}] C -> S {packet.GetPropertyInfoString()}");
                        await Logger.Info<Program>($"[{packet.State}] C -> S 0x{packet.PacketId.ToString("X").PadLeft(2, '0')}");
                    };
                    clientAdapter.UnregisteredPacketReceived += async (_, e) =>
                    {
                        var dataPacket = e.dataPacket;
                        await serverAdapter.SendDataPacket(dataPacket).LogException<Program>();
                        var content = dataPacket.Content;
                        content.Position = 0;
                        var printer = new StreamPrinter(content);
                        await Logger.Info<Program>($"[{dataPacket.State}] C -> S: Unknow packet: 0x{dataPacket.PacketId.ToString("X").PadLeft(2, '0')}");
                        //printer.Print();
                    };
                    var clientTask = clientAdapter.Start();
                    var serverTask = serverAdapter.Start();
                    await clientTask;
                }
                catch (Exception ex)
                {
                    await Logger.Exception<Program>(ex);
                }
                finally
                {
                    client?.Close();
                    server?.Close();
                    await Logger.Info<Program>("C >|< S");
                }
            }
        }
    }
}