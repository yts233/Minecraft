using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Test.OpenGL.Test
{
    internal class ClearRenderer : IRenderable
    {
        void IRenderable.Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}