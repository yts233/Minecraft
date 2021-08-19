namespace Minecraft.Input
{
    public class KeyboardKeyEventArgs
    {
        public KeyboardKeyEventArgs(Keys key, int scanCode, KeyModifiers modifiers, bool isRepeat)
        {
            Key = key;
            ScanCode = scanCode;
            Modifiers = modifiers;
            IsRepeat = IsRepeat;
        }

        public Keys Key { get; }
        public int ScanCode { get; }
        public KeyModifiers Modifiers { get; }
        public bool IsRepeat { get; }
        public bool Alt => (Modifiers & KeyModifiers.Alt) == KeyModifiers.Alt;
        public bool Control => (Modifiers & KeyModifiers.Control) == KeyModifiers.Control;
        public bool Shift => (Modifiers & KeyModifiers.Shift) == KeyModifiers.Shift;
        public bool Command => (Modifiers & KeyModifiers.Super) == KeyModifiers.Super;
    }
}