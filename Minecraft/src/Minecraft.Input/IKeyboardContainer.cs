using System;

namespace Minecraft.Input
{
    public interface IKeyboardContainer
    {
        IKeyboardState KeyboardState { get; }
        event EventHandler<KeyboardKeyEventArgs> KeyDown;
        event EventHandler<KeyboardKeyEventArgs> KeyUp;
    }
}