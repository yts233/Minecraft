using Minecraft;
using Minecraft.Graphics.Rendering;
using System;

namespace Test.OpenGL.ChunkRenderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.SetThreadName("MainThread");
            Logger.GetLogger<Program>().HelloWorld("ChunkRenderTest");
            new MainWindow()
            {
                Title = "Hello ChunkRenderTest"
            }.ReloadWindow();
        }
    }
}
