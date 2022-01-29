//#define RAWDATA

#if RAWDATA
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

            static void LogPacket(byte[] data, int length, bool isServer)
            {
                var @string = new StringBuilder();
                Stream buff = new MemoryStream(data, 0, length, false);
                @string.Append('\n');
                @string.Append(DateTime.Now.ToString("H:mm:ss"));
                @string.Append("\t" + (isServer ? "S->C" : "C->S"));
                @string.Append("\tlength:" + length);
                @string.Append('\n');
                @string.Append("Address  00 11 22 33 44 55 66 77 88 99 AA BB CC DD EE FF  0123456789ABCDEF\n");
                while (buff.Position < buff.Length)
                {
                    var sb = new StringBuilder();
                    @string.Append("0x" + buff.Position.ToString("X").PadLeft(5, '0') + "  ");
                    for (var i = 0; i < 16; i++)
                    {
                        var tmp = buff.ReadByte();
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

                lock (Console.Out)
                    Console.WriteLine(@string);
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
                try
                {
                    server.Connect(new IPEndPoint(IPAddress.Parse(args.Length == 1 ? args[0] : "127.0.0.1"), 25566));
                    //server.Connect(Dns.GetHostAddresses("bgp.polarstar.cc"), 11201);
                    Console.Write(" <-> [/" + server.Client.RemoteEndPoint + "]\n");
                    NetworkStream clientStream = client.GetStream(), serverStream = server.GetStream();
                    Task serverTask = new(() =>
                    {
                        try
                        {
                            Thread.CurrentThread.IsBackground = true;
                            var buffer = new byte[1048576];
                            int s;
                            while ((s = serverStream.Read(buffer, 0, 1048576)) != 0)
                            {
                                var buffer1 = new byte[1048576];
                                buffer.CopyTo(buffer1, 0);
                                Task.Run(() => LogPacket(buffer1, s, true));
                                clientStream.WriteAsync(buffer, 0, s);
                            }

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
                        catch
                        {
                        }
                    }), clientTask = new(() =>
                    {
                        try
                        {
                            Thread.CurrentThread.IsBackground = true;
                            var buffer = new byte[1048576];
                            int s;
                            while ((s = clientStream.Read(buffer, 0, 1048576)) != 0)
                            {
                                LogPacket(buffer, s, false);
                                serverStream.WriteAsync(buffer, 0, s);
                            }

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
                        catch
                        {
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
#else
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Minecraft.Protocol;
using Minecraft.Extensions;
using Minecraft.Protocol.Data;
using Minecraft.Protocol.Packets;
using Minecraft.Protocol.Packets.Server;
using Minecraft.Protocol.Packets.Client;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Minecraft;


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

            static async void LogPacket(IPacket packet, bool isServer)
            {
                await Task.Yield();
                lock (Console.Out)
                {
                    if (packet is DataPacket dataPacket)
                    {
                        if (isServer)
                            return;
                        dataPacket.Content.Position = 0;
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
                    else
                    {
                        var @string = new StringBuilder();
                        @string.Append('\n');
                        @string.Append(DateTime.Now.ToString("H:mm:ss FFFFFFF"));
                        @string.Append("\t" + (isServer ? "S->C" : "C->S"));
                        @string.Append('\t');
                        var s = packet.GetPropertyInfoString();
                        if (s.Length > 256)
                            s = packet.GetType().FullName;
                        @string.Append(s);
                        Console.WriteLine(@string);
                        Console.Out.Flush();
                    }
                }
            }

            mainThread = Thread.CurrentThread;
            mainThread.IsBackground = true;
            var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 25565));
            listener.Start();
            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.Write("[/" + client.Client.RemoteEndPoint + "]");
                //listener.Stop();
                var server = new TcpClient();
                try
                {
                    server.Connect(new IPEndPoint(IPAddress.Parse(args.Length == 1 ? args[0] : "127.0.0.1"), 25566));
                    //server.Connect(Dns.GetHostAddresses("mc.oxygenstudio.cn"), 25565);
                    Console.Write(" <-> [/" + server.Client.RemoteEndPoint + "]\n");
                    Stream clientStream = new BufferedStream(client.GetStream()), serverStream = new BufferedStream(server.GetStream());
                    var clientAdapter = new ProtocolAdapter(clientStream, PacketBoundTo.Client)
                    {
                        AutoSendSpecialPacket = false
                    };
                    var serverAdapter = new ProtocolAdapter(serverStream, PacketBoundTo.Server)
                    {
                        AutoSendSpecialPacket = false
                    };
                    clientAdapter.Start();
                    serverAdapter.Start();
                    //var lockB = new object();
                    Task serverTask = new(() =>
                            {
                                try
                                {
                                    Thread.CurrentThread.IsBackground = true;

                                    while (true)
                                    {
                                        var packet = serverAdapter.ReadPacket();
                                        if (packet == null)
                                            break;
                                        clientAdapter.WritePacket(packet);
                                        LogPacket(packet, true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                                finally
                                {
                                    try
                                    {
                                        Console.Write("[/" + client.Client.RemoteEndPoint + "] >|< [/" +
                                                      server.Client.RemoteEndPoint + "] [S >|< C]\n");
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
                                            var packet = clientAdapter.ReadPacket();
                                            if (packet == null)
                                                break;
                                            serverAdapter.WritePacket(packet);
                                            LogPacket(packet, false);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                    finally
                                    {
                                        try
                                        {
                                            Console.Write("[/" + client.Client.RemoteEndPoint + "] >|< [/" +
                                                          server.Client.RemoteEndPoint + "] [C >|< S]\n");
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
                }
                finally
                {
                    server.Dispose();
                    client.Dispose();
                }
            }
        }
    }
}
#endif