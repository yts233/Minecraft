using Minecraft;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Windowing;
using System;

namespace Test.OpenGL.ChunkRenderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.SetThreadName("MainThread");
            Logger.GetLogger<Program>().HelloWorld("ChunkRenderTest");
            MainWindow window = null;
            RenderWindow.GlfwThread.Start();
            RenderWindow.GlfwThread.Invoke(() =>
            {
                window = new MainWindow
                {
                    Title = "Hello ChunkRenderTest"
                };
            });
            window.Run();
            RenderWindow.GlfwThread.Stop();
        }
    }
}
