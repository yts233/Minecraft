using System;
using OpenTK.Mathematics;

namespace Minecraft.Input
{
    public class PointerMoveEventArgs : EventArgs
    {
        public PointerMoveEventArgs(Vector2 position, Vector2 delta)
        {
            Position = position;
            Delta = delta;
        }

        public Vector2 Position { get; }
        public Vector2 Delta { get; }
    }
}