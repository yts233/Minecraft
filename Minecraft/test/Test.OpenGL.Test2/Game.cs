using Minecraft;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Shading;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Test.OpenGL.Test2
{
    internal class Game : GameWindow
    {
        private IShader shader;
        private IVertexArray vertexArray;

        private Game() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
        }

        private static void Main(string[] args)
        {
            Logger.Info<Game>("Hello World!");
            Logger.SetExceptionHandler();
            new Game().Run();
        }

        protected override void OnLoad()
        {
            float[] vertices =
            {
                -0.5F, -0.5F, .0F, 1.0F, .0F, .0F,
                0.5F, -0.5F, .0F, .0F, 1.0F, .0F,
                0F, 0.5F, .0F, .0F, .0F, 1.0F
            };
            vertexArray = new VertexArray<float>(vertices, new []
            {
                new VertexAttributePointer
                {
                    Index = 0,
                    Normalized = false,
                    Offset = 0,
                    Size = 3,
                    Type = VertexAttribePointerType.Float
                },
                new VertexAttributePointer
                {
                    Index = 1,
                    Normalized = false,
                    Offset = 3 * sizeof(float),
                    Size = 3,
                    Type = VertexAttribePointerType.Float
                }
            });
            shader = new ShaderBuilder()
                .AttachVertexShader(@"
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;
out vec3 Color;
void main() {
    gl_Position = vec4(aPosition, 1F);
    Color = aColor;
}
")
                .AttachFragmentShader(@"
#version 330 core
in vec3 Color;
out vec4 FragColor;

void main() {
    FragColor = vec4(Color,1F);
}
")
                .Link();
            shader.Use();
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shader.Use();
            vertexArray.Render();
            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            vertexArray.Dispose();
            base.OnUnload();
        }
    }
}