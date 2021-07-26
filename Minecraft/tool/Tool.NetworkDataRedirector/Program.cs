using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Minecraft.Protocol.Data;
using Minecraft.Protocol.Packets;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


//test server: bgp.polarstar.cc:11201
// connect bgp.polarstar.cc 11201

namespace Tool.NetworkDataRedirector
{
    internal class Program
    {
        private static Thread mainThread;

        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            static void LogPacket(DataPacket dataPacket, bool isServer)
            {
                lock (Console.Out)
                {
                    var @string = new StringBuilder();
                    var buff = dataPacket.Content;
                    @string.Append('\n');
                    @string.Append(DateTime.Now.ToString("H:mm:ss FFFFFFF"));
                    @string.Append("\t" + (isServer ? "S->C" : "C->S"));
                    @string.Append($"\tpacket: 0x{dataPacket.PacketId.ToString("X").PadLeft(2, '0')}");
                    @string.Append("\tlength:" + buff.Length);
                    @string.Append('\n');
                    void ProcessData(Stream stream)
                    {
                        @string.Append("Address  00 11 22 33 44 55 66 77 88 99 AA BB CC DD EE FF  0123456789ABCDEF\n");
                        while (stream.Position < stream.Length)
                        {
                            var sb = new StringBuilder();
                            @string.Append("0x" + stream.Position.ToString("X").PadLeft(5, '0') + "  ");
                            for (var i = 0; i < 16; i++)
                            {
                                var tmp = stream.ReadByte();
                                if (tmp == -1)
                                {
                                    sb.Append(' ');
                                    @string.Append(".. ");
                                    continue;
                                }

                                sb.Append(tmp >= 32 ? (char)tmp : '.');
                                @string.Append(tmp.ToString("X").PadLeft(2, '0') + " ");
                            }

                            @string.Append(' ');
                            @string.Append(sb);
                            @string.Append('\n');
                        }
                        @string.Append("End Of Data");
                    }
                    ProcessData(buff);
                    Console.WriteLine(@string);
                    Console.Out.Flush();
                }
            }

            mainThread = Thread.CurrentThread;
            mainThread.IsBackground = true;
            var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 25565));
            while (true)
            {
                listener.Start();
                var client = listener.AcceptTcpClient();
                Console.Write("[/" + client.Client.RemoteEndPoint + "]");
                listener.Stop();
                var server = new TcpClient();
                var compressed = false;
                try
                {
                    server.Connect(new IPEndPoint(IPAddress.Parse(args.Length == 1 ? args[0] : "127.0.0.1"), 25566));
                    //server.Connect(Dns.GetHostAddresses("bgp.polarstar.cc"), 11201);
                    Console.Write(" <-> [/" + server.Client.RemoteEndPoint + "]\n");
                    Stream clientStream = new BufferedStream(client.GetStream()), serverStream = new BufferedStream(server.GetStream());
                    Task serverTask = new(() =>
                    {
                        try
                        {
                            Thread.CurrentThread.IsBackground = true;

                            while (true)
                            {
                                var dataPacket = Packet.ReadDataPacket(serverStream, PacketBoundTo.Client, () => Minecraft.Protocol.ProtocolState.Any, () => compressed);
                                if (dataPacket.PacketId == 0x03) compressed = true;
                                Task.Run(() =>
                                {
                                    if (compressed) dataPacket.WriteCompressedToStream(clientStream);
                                    else dataPacket.WriteToStream(clientStream);
                                    clientStream.Flush();
                                    dataPacket.Content.Position = 0;
                                    LogPacket(dataPacket, true);
                                });
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            try
                            {
                                Console.Write("[/" + client.Client.RemoteEndPoint + "] >|< [/" +
                                              server.Client.RemoteEndPoint + "]\n");
                            }
                            catch
                            {
                            }

                            if (client.Connected) client.Close();
                            if (server.Connected) server.Close();
                        }
                    }), clientTask = new(() =>
                        {
                            try
                            {
                                Thread.CurrentThread.IsBackground = true;
                                while (true)
                                {
                                    var dataPacket = Packet.ReadDataPacket(clientStream, PacketBoundTo.Server, () => Minecraft.Protocol.ProtocolState.Any, () => compressed);
                                    if (dataPacket.PacketId == 0x03) compressed = true;
                                    Task.Run(() =>
                                    {
                                        if (compressed) dataPacket.WriteCompressedToStream(serverStream);
                                        else dataPacket.WriteToStream(serverStream);
                                        serverStream.Flush();
                                        dataPacket.Content.Position = 0;
                                        LogPacket(dataPacket, false);
                                    });
                                }
                            }
                            catch
                            {
                            }
                            finally
                            {
                                try
                                {
                                    Console.Write("[/" + client.Client.RemoteEndPoint + "] >|< [/" +
                                                  server.Client.RemoteEndPoint + "]\n");
                                }
                                catch
                                {
                                    // ignore
                                }

                                if (client.Connected) client.Close();
                                if (server.Connected) server.Close();
                            }
                        });
                    serverTask.Start();
                    clientTask.Start();
                    serverTask.Wait();
                    clientTask.Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    if (client.Connected) client.Close();
                    if (server.Connected) server.Close();
                }
            }
        }
    }
}