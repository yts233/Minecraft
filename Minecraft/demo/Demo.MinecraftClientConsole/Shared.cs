using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Minecraft;
//test server: mc.oxygenstudio.cn
// connect mc.oxygenstudio.cn
// connect s1.zhaomc.net
namespace Demo.MinecraftClientConsole
{
    class Shared
    {
        private static Queue<(Action action, Action callback)> _graphicsThreadDelegates = new();
        private static bool _graphicsThreadStarted = false;
        private static Logger<Shared> _logger = Logger.GetLogger<Shared>();

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

        public static IThreadDispatcher RenderThread { get; } = ThreadHelper.CreateDispatcher("RenderThread");
    }
}