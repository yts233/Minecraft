using Minecraft;
using Minecraft.Graphics.Windowing;
using System;

namespace Test.MinecraftClientAndOpenGL.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.GetLogger<Program>().HelloWorld();
            var window = RenderWindow.InvokeOnGlfwThread(() => new MainWindow("s1.zhaomc.net"));
            while (true)
                window.Run();
        }
    }
}
