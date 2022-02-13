using Minecraft;
using Minecraft.Graphics.Windowing;
using System;

namespace Demo.MCGraphics2D
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger.SetExceptionHandler();
            Console.WriteLine("Hello World!");
            RenderWindow.InvokeOnGlfwThread(() => new MainWindow()).Run();

            Logger.WaitForLogging();
        }
    }
}
