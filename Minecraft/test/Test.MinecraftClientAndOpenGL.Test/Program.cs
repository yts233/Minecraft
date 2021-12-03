using Minecraft;
using Minecraft.Graphics.Windowing;
using System;

namespace Test.MinecraftClientAndOpenGL.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.GetLogger<Program>().HelloWorld("MinecraftClientAndOpenGL");
            var window = SimpleRenderWindowContainer.InvokeOnGlfwThread(() =>
            {
                Console.WriteLine("type player name below.");
                return new MainWindow(Console.ReadLine());
            });
            while(true)
            {
                Console.WriteLine("the window was closed.");
                Console.ReadKey();
                window.Run();
            }
        }
    }
}
