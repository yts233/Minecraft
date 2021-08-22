using System;

namespace Minecraft.Graphics.Rendering
{
    public class CustomInitializer : IInitializer
    {
        private readonly Action _action;

        public CustomInitializer(Action action)
        {
            _action = action;
        }

        public void Initialize()
        {
            _action?.Invoke();
        }
    }
}