using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Utils
{
    public class ViewportRenderer : IRenderable
    {
        private readonly IRenderContainer _container;
        private Vector2i _prevSize;

        public ViewportRenderer(IRenderContainer container)
        {
            _container = container;
        }

        public void Render()
        {
            if (_prevSize != _container.ClientSize)
            {
                _prevSize = _container.ClientSize;
                GL.Viewport(0, 0, _prevSize.X, _prevSize.Y);
            }
        }
    }
}
