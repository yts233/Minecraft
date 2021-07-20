using System;
using System.Collections.Generic;
using System.Linq;
using Minecraft;
using Minecraft.Client;

namespace Demo.MinecraftClientConsole
{
    class Program
    {
        private delegate void Command(string[] args);

        private delegate void CmdLet();

        private static readonly Dictionary<string, (Command command, string help)> Commands =
            new();

        private static MinecraftClient _client;

        public static void Main(string[] args)
        {
            Logger.SetThreadName("MainThread");
            Logger.SetExceptionHandler();
            Logger.Info<Program>("Hello, Minecraft Client Console!");
            _client = new MinecraftClient("MCConsoleTest");
            LoadCommands();
            Logger.Info<Program>("type /help for commands");
            while (true)
            {
                //Console.Write("CmdLet> ");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    continue;
                input = input.Trim();
                if (input.StartsWith('/'))
                {
                    var cmd = input.Split(' ');
                    var commandName = cmd[0];
                    var commandParams = cmd.Skip(1).ToArray();
                    if (!Commands.ContainsKey(commandName))
                    {
                        Logger.Error<CmdLet>($"unknown command: {commandName}, type /help for commands");
                        continue;
                    }

                    try
                    {
                        Commands[commandName].command(commandParams);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception<CmdLet>(ex);
                    }

                    continue;
                }

                Logger.Info<Program>(input);
            }
        }

        private static void AddCommand(string commandName, Command command, string help)
        {
            Commands.Add(commandName, (command, help));
        }

        private static void Output(object obj, ConsoleColor? color = null)
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

        private static void ShowHelp(string commandName)
        {
            var (_, help) = Commands[commandName];
            Output($"{commandName} {help}", ConsoleColor.Yellow);
        }

        private static void LoadCommands()
        {
            AddCommand("/help", _ =>
            {
                foreach (var commandName in Commands.Keys) ShowHelp(commandName);
            }, "获取帮助");
            AddCommand("/ping", async args =>
            {
                var defaultPort = false;
                if (args.Length == 1)
                {
                    defaultPort = true;
                }
                else if (args.Length != 2)
                {
                    ShowHelp("/ping");
                    return;
                }

                var hostname = args[0];
                var port = defaultPort ? (ushort) 25565 : ushort.Parse(args[1]);

                var result = await _client.ServerListPingAsync(hostname, port);

                Logger.Info<CmdLet>($"from {hostname}:{port}, ping: {result.Delay}ms\n{result.Content}");
            }, "<hostname> [port=25565] 请求一次服务器列表ping");
            AddCommand("/clear", _ => Console.Clear(), "清空控制台缓冲区");
            AddCommand("/exit", _ => { Environment.Exit(0); }, "退出此 Minecraft Client Console");
        }
    }
}