using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minecraft;
using Minecraft.Numerics;
using Minecraft.Extensions;
using Minecraft.Client;
using static Demo.MinecraftClientConsole.Shared;
using Demo.MinecraftClientConsole.Graphics;
using Minecraft.Graphics.Windowing;
//test server: mc.oxygenstudio.cn
// connect mc.oxygenstudio.cn
// connect s1.zhaomc.net
namespace Demo.MinecraftClientConsole
{

    class Program
    {
        private delegate void Command(string[] args);

        private delegate void CmdLet();

        private static readonly Dictionary<string, (Command command, string help)> Commands =
            new();

        private static MinecraftClient _client;
        private static readonly Logger<CmdLet> _cmdLetLogger = Logger.GetLogger<CmdLet>();

        public static void CreateClient(string userName)
        {
            //initalize client
            _client = new MinecraftClient(userName);
            _client.Disconnected += (_, e) => Console.WriteLine($"Disconnected. Reason: {e}");
            _client.ChatReceived += (_, e) => Console.WriteLine(e);
        }

        public static void Main(string[] args)
        {
            Logger.SetThreadName("MainThread");
            Logger.SetExceptionHandler();
            Println("Hello, Minecraft Client Console!");
            CreateClient("MCClientTest");
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

                if (input.StartsWith('/'))
                {
                    _client.SendChatMessage(input);
                    continue;
                }

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
            AddCommand("help", args =>
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
            }, "[command] 获取帮助");
            AddCommand("echo", args =>
            {
                Println(string.Join(' ', args));
            }, "回声输出");
            AddCommand("newclient", args =>
            {
                if (args.Length > 1)
                {
                    ShowHelp("newclient");
                    return;
                }
                var userName = "MCConsoleTest";
                if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
                    userName = args[0];
                _client.Disconnect();
                CreateClient(userName);
            }, "[username=MCConsoleTest] 重新创建MinecraftClient实例");
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
                    ShowHelp("connect");
                    return;
                }

                var hostname = args[0];
                var port = defaultPort ? (ushort)25565 : ushort.Parse(args[1]);
                _client.Connect(hostname, port);
                /*_client.GetAdapter().PlayerPosition += (_, e) =>
                {
                    e.position
                };*/
            }, "<hostname> [port=25565] 连接到服务器，并切换到 InServer");
            AddCommand("disconnect", args =>
            {
                _client.Disconnect();
            }, "断开与服务器的连接");
            AddCommand("reconnect", args =>
            {
                _client.Reconnect();
            }, "重新连接到服务器");
            AddCommand("send", args =>
            {
                var msg = string.Join(' ', args);
                _client.SendChatMessage(msg);
            }, "<message> 向服务器发送聊天或指令");

            #region Packets
            AddCommand("vehiclemove", args =>
            {
                if (args.Length == 5
                && double.TryParse(args[0], out var x)
                && double.TryParse(args[1], out var y)
                && double.TryParse(args[2], out var z)
                && float.TryParse(args[3], out var yaw)
                && float.TryParse(args[4], out var pitch))
#pragma warning disable CS0618 // 类型或成员已过时
                    _client.GetAdapter().SendVehicleMovePacket((x, y, z), (yaw, pitch));
#pragma warning restore CS0618 // 类型或成员已过时
                else ShowHelp("vehiclemove");
            }, "<x> <y> <z> <yaw> <pitch> 向服务器发送VehicleMovePacket");
            AddCommand("playerposition", args =>
            {
                if (args.Length == 4
                && double.TryParse(args[0], out var x)
                && double.TryParse(args[1], out var feetY)
                && double.TryParse(args[2], out var z)
                && bool.TryParse(args[3], out var onGround))
#pragma warning disable CS0618 // 类型或成员已过时
                    _client.GetAdapter().SendPlayerPositionPacket((x, feetY, z), onGround);
#pragma warning restore CS0618 // 类型或成员已过时
                else ShowHelp("playerposition");
            }, "<x> <feety> <z> <onGround> 向服务器发送PlayerPositionPacket");
            #endregion

            #region InGame
            AddCommand("entities", _ =>
            {
                if (!_client.IsJoined)
                    return;
                foreach (var entity in _client.GetWorld().GetEntities())
                {
                    Console.WriteLine(entity.GetPropertyInfoString());
                }
            }, "获取所有实体");
            AddCommand("interactentity", args =>
            {
                if (!_client.IsJoined)
                    return;
                if (args.Length == 0)
                {
                    ShowHelp("interactentity");
                    return;
                }
                if (!int.TryParse(args[0], out var entityId))
                {
                    ShowHelp("interactentity");
                    return;
                }
                var entity = _client.GetWorld().GetEntities().FirstOrDefault(e => e.EntityId == entityId);
                if (entity == null)
                {
                    Println("找不到指定的实体！", ConsoleColor.Red);
                    return;
                }
                switch (args.Length)
                {
                    case 1:
                        _client.GetPlayer().Attack(entity, false);
                        break;
                    case 2:
                        {
                            if (!int.TryParse(args[1], out var hand))
                            {
                                ShowHelp("interactentity");
                                return;
                            }
                            _client.GetPlayer().Interact(entity, (Hand)hand, false);
                            break;
                        }

                    case 5:
                        {
                            if (!float.TryParse(args[1], out var x) || !float.TryParse(args[1], out var y) || !float.TryParse(args[1], out var z) || !int.TryParse(args[1], out var hand))
                            {
                                ShowHelp("interactentity");
                                return;
                            }
                            _client.GetPlayer().Interact(entity, (x, y, z), (Hand)hand, false);
                            break;
                        }

                    default:
                        ShowHelp("interactentity");
                        return;
                }
            }, "{<entityid> 攻击实体} | [target: x y z] <hand> 与实体交互");
            #endregion

            #region Graphics

            AddCommand("graphicspositioncontroller", _ =>
            {
                static void GraphicsThread()
                {
                    SimpleRenderWindowContainer.InvokeOnGlfwThread(() => new ClientPositionController(_client)).Run();
                }

                ThreadHelper.StartThread(GraphicsThread, "GraphicsPositionControllerThread");
            }, "创建图形化坐标控制器");

            #endregion

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
}