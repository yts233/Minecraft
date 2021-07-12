using System.Drawing;
using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Test.OpenGL.Test
{
    internal class WindowInitializer : IInitializer
    {
        public void Initialize()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);
            //GL.Enable(EnableCap.CullFace);
            GL.ClearColor(Color.CornflowerBlue);
        }
    }
}