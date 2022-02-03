using Minecraft;
using Minecraft.Graphics.Windowing;
using System;

namespace Demo.MCGraphicsCloud
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger.SetExceptionHandler();
            Console.WriteLine("Hello World!");

            // create a window and run
            RenderWindow.InvokeOnGlfwThread(() => new MainWindow()).Run();

            Logger.WaitForLogging();
        }
    }
}
