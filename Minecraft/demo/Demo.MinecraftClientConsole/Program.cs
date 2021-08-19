using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minecraft;
using Minecraft.Extensions;
using Minecraft.Client;
using static Demo.MinecraftClientConsole.Shared;
//test server: bgp.polarstar.cc:11201
// connect bgp.polarstar.cc 11201
namespace Demo.MinecraftClientConsole
{
    static class Shared
    {
        public static void Print(object obj, ConsoleColor? color = null)
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

        public static void Println(object obj, ConsoleColor? color = null)
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
    }

    class Program
    {
        private delegate void Command(string[] args);

        private delegate void CmdLet();

        private static readonly Dictionary<string, (Command command, string help)> Commands =
            new();

        private static MinecraftClient _client;
        private static readonly Logger<CmdLet> _cmdLetLogger = Logger.GetLogger<CmdLet>();

        public static void Main(string[] args)
        {
            Logger.SetThreadName("MainThread");
            Logger.SetExceptionHandler();
            Println("Hello, Minecraft Client Console!");
            _client = new MinecraftClient("MCConsoleTest");
            LoadCommands();
            Println("type help for commands");
            Logger.WaitForLogging();
            while (true)
            {
                Logger.WaitForLogging();
                //int cursorTop;
                lock (Console.Out)
                {
                    //cursorTop = Console.CursorTop;
                    Print("CmdLet> ", ConsoleColor.Green);
                }
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;
                //Console.CursorTop = cursorTop;
                //Console.CursorLeft = 8;
                //Console.Write(string.Empty.PadLeft(input.Length));
                input = input.Trim();
                //Console.CursorTop = cursorTop;
                //Console.CursorLeft = 8;
                //Println(input, ConsoleColor.Yellow);

                var cmd = input.Split(' ');
                var commandName = cmd[0];
                var commandParams = cmd.Skip(1).ToArray();
                if (!Commands.ContainsKey(commandName))
                {
                    Println($"unknown command: {commandName}, type help for commands", ConsoleColor.Red);
                    continue;
                }

                try
                {
                    Commands[commandName].command(commandParams);
                }
                catch (Exception ex)
                {
                    _cmdLetLogger.Error(ex);
                }
            }
        }

        private static void AddCommand(string commandName, Command command, string help)
        {
            Commands.Add(commandName, (command, help));
        }

        private static void ShowHelp(string commandName)
        {
            var (_, help) = Commands[commandName];
            Println($"{commandName} {help}", ConsoleColor.Yellow);
        }

        private static void LoadCommands()
        {
            AddCommand("help", _ =>
            {
                Println("Minecraft Client Console 命令", ConsoleColor.Yellow);
                foreach (var commandName in Commands.Keys) ShowHelp(commandName);
            }, "获取帮助");
            AddCommand("echo", args =>
            {
                Println(string.Join(' ', args));
            }, "回声输出");
            //AddCommand("username", async args =>
            //{
            //    if (args.Length != 1)
            //    {
            //        ShowHelp("username");
            //        return;
            //    }
            //    _client.ChangeOfflineUserName(args[0]);
            //    await Task.CompletedTask;
            //}, "<username> 改变离线用户名");
            AddCommand("protocol", args =>
            {
                if (args.Length != 1)
                {
                    ShowHelp("protocol");
                    return;
                }
                var protocolNumber = int.Parse(args[0]);
                _client.SwitchProtocolVersion(protocolNumber);
            }, "<version_number> 改变协议版本号");
            AddCommand("ping", args =>
            {
                var defaultPort = false;
                if (args.Length == 1)
                {
                    defaultPort = true;
                }
                else if (args.Length != 2)
                {
                    ShowHelp("ping");
                    return;
                }

                var hostname = args[0];
                var port = defaultPort ? (ushort)25565 : ushort.Parse(args[1]);

                var result = _client.ServerListPing(hostname, port);

                Println($"from {hostname}:{port}, ping: {result.Delay}ms\n游戏版本: {result.VersionName}\n协议版本号: {result.ProtocolVersion}\n在线人数: {result.OnlinePlayerCount}/{result.MaxPlayerCount}\n{result.Description}");
            }, "<hostname> [port=25565] 请求一次服务器列表ping");
            AddCommand("connect", args =>
            {
                var defaultPort = false;
                if (args.Length == 1)
                {
                    defaultPort = true;
                }
                else if (args.Length != 2)
                {
                    ShowHelp("ping");
                    return;
                }

                var hostname = args[0];
                var port = defaultPort ? (ushort)25565 : ushort.Parse(args[1]);
                //_ = _client.Connect(hostname, port);
                new InServer(_client).Main();
            }, "连接到服务器，并切换到 InServer");
            //AddCommand("logger", args =>
            //{
            //    if (args.Length != 2)
            //    {
            //        ShowHelp("loglevel");
            //        return;
            //    }

            //    switch (args[0])
            //    {
            //        case "enable":
            //            Logger.EnableLogger(args[1]);
            //            break;
            //        case "disable":
            //            Logger.DisableLogger(args[1]);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(null, nameof(args));
            //    };

            //}, "<enable|disable> <loggerName>");
            //AddCommand("loglevel", async args =>
            //{
            //    if (args.Length != 2)
            //    {
            //        ShowHelp("loglevel");
            //        return;
            //    }

            //    var enable = args[0] switch
            //    {
            //        "enable" => true,
            //        "disable" => false,
            //        _ => throw new ArgumentOutOfRangeException(null, nameof(args)),
            //    };

            //    var level = (LogLevel)int.Parse(args[1]);
            //    Logger.SetLogLevel(level, enable);

            //    await Task.CompletedTask;
            //}, "<enable|disable> <level:{0:fatal|1:error|2:warn|3:info|4:debug}> 启用或禁用记录器记录指定等级的日志");
            AddCommand("clear", _ =>
            {
                Console.Clear();
            }, "清空控制台缓冲区");
            AddCommand("exit", _ =>
            {
                Environment.Exit(0);
            }, "退出此 Minecraft Client Console");
        }
    }
    class InServer
    {
        private readonly MinecraftClient _client;

        public InServer(MinecraftClient client)
        {
            _client = client;
        }

        public void Main()
        {
            while (_client.IsConnected)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;
                input = input.Trim();
                if (input == "#exit")
                    break;
                //_client.Chat(input);
            }
            //_client.Disconnect();
        }
    }
}