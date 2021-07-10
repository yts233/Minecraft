using System;

namespace Minecraft.Graphics.Rendering
{
    public class CustomTicker : ITickable
    {
        private readonly Action _action;

        public CustomTicker(Action action)
        {
            _action = action;
        }

        public void Tick()
        {
            _action?.Invoke();
        }
    }
}