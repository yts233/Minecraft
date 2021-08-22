using System;
using Minecraft.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Minecraft.Graphics.Windowing
{
    internal class WindowPointerState : IPointerState
    {
        private readonly MouseState _mouseState;

        public WindowPointerState(MouseState mouseState)
        {
            _mouseState = mouseState ?? throw new ArgumentNullException(nameof(mouseState));
        }

        public bool this[PointerButton button] => _mouseState[(MouseButton) button];

        public Vector2 Position { get; set; }

        public Vector2 PreviousPosition { get; set; }

        public Vector2 Delta { get; set; }

        public bool IsAnyButtonDown => _mouseState.IsAnyButtonDown;

        public bool IsButtonDown(PointerButton button)
        {
            return _mouseState.IsButtonDown((MouseButton) button);
        }

        public bool WasButtonDown(PointerButton button)
        {
            return _mouseState.WasButtonDown((MouseButton) button);
        }

        public override bool Equals(object obj)
        {
            return obj is WindowPointerState mousePointerState && mousePointerState._mouseState == _mouseState;
        }

        public override int GetHashCode()
        {
            return _mouseState.GetHashCode();
        }
    }
}