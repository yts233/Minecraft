using System;

namespace Minecraft.Input
{
    public interface IKeyboardContainer
    {
        IKeyboardState KeyboardState { get; }
        event Action<KeyboardKeyEventArgs> KeyDown;
        event Action<KeyboardKeyEventArgs> KeyUp;
    }
}