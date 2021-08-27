using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minecraft;
using Minecraft.Numerics;
using Minecraft.Extensions;
using Minecraft.Client;
using static Demo.MinecraftClientConsole.Shared;
//test server: mc.oxygenstudio.cn
// connect mc.oxygenstudio.cn
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
        private delegate Task Command(string[] args);

        private delegate void CmdLet();

        private static readonly Dictionary<string, (Command command, string help)> Commands =
            new();

        private static MinecraftClient _client;
        private static readonly Logger<CmdLet> _cmdLetLogger = Logger.GetLogger<CmdLet>();

        public static async Task Main(string[] args)
        {
            Logger.SetThreadName("MainThread");
            Logger.SetExceptionHandler();
            Println("Hello, Minecraft Client Console!");
            //initalize client
            _client = new MinecraftClient("MCConsoleTest");
            _client.Disconnected += (_, e) => Console.WriteLine($"Disconnected. Reason: {e}");
            LoadCommands();
            Println("type help for commands");
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
                    await Commands[commandName].command(commandParams);
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
            AddCommand("help", async args =>
            {
                if (args.Length > 1)
                {
                    ShowHelp("help");
                    return;
                }
                else if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
                {
                    var prefix = args[0];
                    Println("Minecraft Client Console 命令", ConsoleColor.Yellow);
                    foreach (var commandName in Commands.Keys.Where(key => key.StartsWith(prefix)))
                        ShowHelp(commandName);
                    return;
                }
                Println("Minecraft Client Console 命令", ConsoleColor.Yellow);
                foreach (var commandName in Commands.Keys) ShowHelp(commandName);
                await Task.CompletedTask;
            }, "[command] 获取帮助");
            AddCommand("echo", async args =>
            {
                Println(string.Join(' ', args));
                await Task.CompletedTask;
            }, "回声输出");
            AddCommand("newclient", async args =>
            {
                if (args.Length > 1)
                {
                    ShowHelp("newclient");
                    return;
                }
                var userName = "MCConsoleTest";
                if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
                    userName = args[0];
                await _client.Disconnect();
                _client = new MinecraftClient(userName);
                await Task.CompletedTask;
            }, "[username=MCConsoleTest] 重新创建MinecraftClient实例");
            AddCommand("protocol", async args =>
            {
                if (args.Length != 1)
                {
                    ShowHelp("protocol");
                    return;
                }
                var protocolNumber = int.Parse(args[0]);
                _client.SwitchProtocolVersion(protocolNumber);
                await Task.CompletedTask;
            }, "<version_number> 改变协议版本号");
            AddCommand("ping", async args =>
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

                var result = await _client.ServerListPing(hostname, port);

                Println($"from {hostname}:{port}, ping: {result.Delay}ms\n游戏版本: {result.VersionName}\n协议版本号: {result.ProtocolVersion}\n在线人数: {result.OnlinePlayerCount}/{result.MaxPlayerCount}\n{result.Description}");
            }, "<hostname> [port=25565] 请求一次服务器列表ping");
            AddCommand("connect", async args =>
            {
                var defaultPort = false;
                if (args.Length == 1)
                {
                    defaultPort = true;
                }
                else if (args.Length != 2)
                {
                    ShowHelp("connect");
                    return;
                }

                var hostname = args[0];
                var port = defaultPort ? (ushort)25565 : ushort.Parse(args[1]);
                await _client.Connect(hostname, port);
                /*_client.GetAdapter().PlayerPosition += (_, e) =>
                {
                    e.position
                };*/
            }, "<hostname> [port=25565] 连接到服务器，并切换到 InServer");
            AddCommand("send", async args =>
            {
                var msg = string.Join(' ', args);
                await _client.Chat(msg);
            }, "<message> 向服务器发送聊天或指令");

            #region Packets
            AddCommand("vehiclemove", async args =>
            {
                if (args.Length == 5
                && double.TryParse(args[0], out var x)
                && double.TryParse(args[1], out var y)
                && double.TryParse(args[2], out var z)
                && float.TryParse(args[3], out var yaw)
                && float.TryParse(args[4], out var pitch))
#pragma warning disable CS0618 // 类型或成员已过时
                    await _client.GetAdapter().SendVehicleMovePacket((x, y, z), (yaw, pitch));
#pragma warning restore CS0618 // 类型或成员已过时
                else ShowHelp("vehiclemove");
            }, "<x> <y> <z> <yaw> <pitch> 向服务器发送VehicleMovePacket");
            AddCommand("playerposition", async args =>
            {
                if (args.Length == 4
                && double.TryParse(args[0], out var x)
                && double.TryParse(args[1], out var feetY)
                && double.TryParse(args[2], out var z)
                && bool.TryParse(args[3], out var onGround))
#pragma warning disable CS0618 // 类型或成员已过时
                    await _client.GetAdapter().SendPlayerPositionPacket((x, feetY, z), onGround);
#pragma warning restore CS0618 // 类型或成员已过时
                else ShowHelp("playerposition");
            }, "<x> <feety> <z> <onGround> 向服务器发送PlayerPositionPacket");
            #endregion

            #region InGame
            AddCommand("entities", async _ =>
            {
                if (!_client.IsJoined)
                    return;
                foreach (var entity in _client.GetWorld().GetEntities())
                {
                    Console.WriteLine(entity.GetPropertyInfoString());
                }
                await Task.CompletedTask;
            }, "获取所有实体");
            #endregion

            AddCommand("clear", async _ =>
            {
                Console.Clear();
                await Task.CompletedTask;
            }, "清空控制台缓冲区");
            AddCommand("exit", async _ =>
            {
                Environment.Exit(0);
                await Task.CompletedTask;
            }, "退出此 Minecraft Client Console");
        }
    }
}