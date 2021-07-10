using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.NetworkDataRedirector
{
    internal class Program
    {
        private static Thread mainThread;

        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Action<byte[], int, bool> logPacket = (data, length, isServer) =>
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

                        sb.Append(tmp >= 32 ? (char) tmp : '.');
                        @string.Append(tmp.ToString("X").PadLeft(2, '0') + " ");
                    }

                    @string.Append(' ');
                    @string.Append(sb);
                    @string.Append('\n');
                }

                Console.WriteLine(@string);
            };
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
                    server.Connect(new IPEndPoint(IPAddress.Parse(args.Length == 1 ? args[0] : "127.0.0.1"), 25577));
                    Console.Write(" <-> [/" + server.Client.RemoteEndPoint + "]\n");
                    NetworkStream clientStream = client.GetStream(), serverStream = server.GetStream();
                    Task serverTask = new Task(() =>
                        {
                            try
                            {
                                Thread.CurrentThread.IsBackground = true;
                                var buffer = new byte[1048576];
                                int s;
                                while ((s = serverStream.Read(buffer, 0, 1048576)) != 0)
                                {
                                    logPacket(buffer, s, true);
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
                        }),
                        clientTask = new Task(() =>
                        {
                            try
                            {
                                Thread.CurrentThread.IsBackground = true;
                                var buffer = new byte[1048576];
                                int s;
                                while ((s = clientStream.Read(buffer, 0, 1048576)) != 0)
                                {
                                    logPacket(buffer, s, false);
                                    serverStream.WriteAsync(buffer, 0, s);
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