using System;

namespace Minecraft.Graphics.Rendering
{
    public class CustomRenderer : IRenderable
    {
        private readonly Action _action;

        public CustomRenderer(Action action)
        {
            _action = action;
        }

        public void Render()
        {
            _action?.Invoke();
        }
    }
}