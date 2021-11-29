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
            Logger.SetExceptionHandler();
            Logger.SetThreadName("MainThread");
            Logger.GetLogger<Program>().HelloWorld("ChunkRenderTest");
            MainWindow window = null;
            SimpleRenderWindowContainer.GlfwThread.Start();
            SimpleRenderWindowContainer.GlfwThread.Invoke(() =>
            {
                window = new MainWindow
                {
                    Title = "Hello ChunkRenderTest"
                };
            });
            window.Run();
            SimpleRenderWindowContainer.GlfwThread.Stop();
        }
    }
}
