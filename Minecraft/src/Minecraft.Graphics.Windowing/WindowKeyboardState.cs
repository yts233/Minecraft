using System;
using Minecraft.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using MKeys = Minecraft.Input.Keys;

namespace Minecraft.Graphics.Windowing
{
    internal class WindowKeyboardState : IKeyboardState
    {
        private readonly KeyboardState _keyboardState;

        public WindowKeyboardState(KeyboardState keyboardState)
        {
            _keyboardState = keyboardState ?? throw new ArgumentNullException(nameof(keyboardState));
        }

        public bool this[MKeys key] => _keyboardState[(Keys) key];

        public bool IsAnyKeyDown => _keyboardState.IsAnyKeyDown;

        public bool IsKeyDown(MKeys key)
        {
            return _keyboardState.IsKeyDown((Keys) key);
        }

        public bool IsKeyPressed(MKeys key)
        {
            return _keyboardState.IsKeyPressed((Keys) key);
        }

        public bool IsKeyReleased(MKeys key)
        {
            return _keyboardState.IsKeyReleased((Keys) key);
        }

        public bool WasKeyDown(MKeys key)
        {
            return _keyboardState.WasKeyDown((Keys) key);
        }
    }
}