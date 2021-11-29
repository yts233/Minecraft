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
            var window = SimpleRenderWindowContainer.InvokeOnGlfwThread(() => new MainWindow());
            while (true)
                window.Run();
        }
    }
}
