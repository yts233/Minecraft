namespace Minecraft.Input
{
    public interface IKeyboardState
    {
        bool this[Keys key] { get; }
        bool IsAnyKeyDown { get; }
        bool IsKeyDown(Keys key);
        bool IsKeyPressed(Keys key);
        bool IsKeyReleased(Keys key);
        bool WasKeyDown(Keys key);
    }
}