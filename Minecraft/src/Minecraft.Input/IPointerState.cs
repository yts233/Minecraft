using OpenTK.Mathematics;

namespace Minecraft.Input
{
    public interface IPointerState
    {
        bool this[PointerButton button] { get; }
        Vector2 Position { get; }
        Vector2 PreviousPosition { get; }
        Vector2 Delta { get; }
        bool IsAnyButtonDown { get; }
        bool IsButtonDown(PointerButton button);
        bool WasButtonDown(PointerButton button);
    }
}