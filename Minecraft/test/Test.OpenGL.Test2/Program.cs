using Minecraft;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Texturing;
using Minecraft.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Threading;

namespace Test.OpenGL.Test2
{
    class Program : GameWindow
    {
        private ImageTexture2D _imageTexture;
        private TestShader _shader;
        private IElementArrayHandle _eah;
        private readonly IFilePath _filepath = ((IFilePath)new FilePath()).Up.Up.Up;

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {

        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.CornflowerBlue);

            using var stream = _filepath["test.png"].OpenRead();
            var image = new Image(stream);
            _imageTexture = new ImageTexture2D(image);
            _shader = new TestShader();
            _eah = new TestVertexProvider().ToElementArray().GetHandle();

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            _imageTexture.Bind();
            _eah.Render();

            SwapBuffers();
        }

        static void Main()
        {
            Logger.SetThreadName("MainThread");
            new Program().Run();
        }
    }
}