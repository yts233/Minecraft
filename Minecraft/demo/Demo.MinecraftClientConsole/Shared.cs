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

        public static void StartGraphicsThread()
        {
            _logger.Info("Started graphics thread");
            new Thread(() =>
            {
                while (true)
                {
                    while (_graphicsThreadDelegates.TryDequeue(out var ele))
                    {
                        ele.action?.Invoke();
                        ele.callback?.Invoke();
                    }
                    Thread.Sleep(1); //cpu break
                }
            })
            { Name = "GraphicsThread" }.Start();
            _graphicsThreadStarted = true;
        }

        public static void RunOnGraphicsThread(Action action, Action callback = null)
        {
            if (!_graphicsThreadStarted)
                StartGraphicsThread();
            _graphicsThreadDelegates.Enqueue((action, callback));
        }

        public static async Task RunOnGraphicsThreadAsync(Action action)
        {
            await Task.Yield();
            if (!_graphicsThreadStarted)
                StartGraphicsThread();
            var source = new TaskCompletionSource();
            _graphicsThreadDelegates.Enqueue((action, () =>
            {
                source.SetResult();
            }
            ));
            await source.Task;
        }
    }
}