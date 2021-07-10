using System;

namespace Minecraft.Graphics.Rendering
{
    public class CustomUpdater : IUpdatable
    {
        private readonly Action _action;

        public CustomUpdater(Action action)
        {
            _action = action;
        }

        public void Update()
        {
            _action?.Invoke();
        }
    }
}