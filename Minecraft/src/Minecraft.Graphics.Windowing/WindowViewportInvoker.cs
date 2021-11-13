using System;
using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Windowing
{
    public class WindowViewportInvoker : IRenderable, IUpdatable, IInitializer
    {
        private readonly bool _auto;
        private readonly RenderWindow _window;
        private bool _changed;
        private Vector2i _location;
        private Vector2i _size;

        public WindowViewportInvoker(RenderWindow window)
        {
            _window = window;
            _auto = true;
        }

        public Vector2i Location
        {
            get => _location;
            set
            {
                if (_changed || (_changed = _location != value))
                    _location = value;
            }
        }

        public Vector2i Size
        {
            get => _size;
            set
            {
                if (_changed || (_changed = _size != value))
                {
                    _size = value;
                    SizeChanged?.Invoke(value);
                }
            }
        }

        void IInitializer.Initialize()
        {
            if (_auto)
            {
                Location = (0, 0);
                Size = _window.ClientSize;
            }
        }

        void IRenderable.Render()
        {
            if (_changed)
            {
                GL.Viewport(_location.X, _location.Y, _size.X, _size.Y);
                _changed = false;
            }
        }

        void IUpdatable.Update()
        {
            if (_auto)
            {
                Location = (0, 0);
                Size = _window.ClientSize;
            }
        }

        public event Action<Vector2i> SizeChanged;
    }
}