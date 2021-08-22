using System;

namespace Minecraft.Input
{
    public class PointerButtonEventArgs : EventArgs
    {
        public PointerButtonEventArgs(PointerButton button, InputAction action, KeyModifiers modifiers)
        {
            Action = action;
            Button = button;
            Modifiers = modifiers;
        }

        public InputAction Action { get; }
        public PointerButton Button { get; }
        public KeyModifiers Modifiers { get; }
        public bool IsPressed => Action != InputAction.Release;
    }
}