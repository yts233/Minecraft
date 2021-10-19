using OpenTK.Mathematics;
using System;

namespace Minecraft.Input
{
    public interface IPointerContainer
    {
        Vector2i ClientSize { get; }
        IPointerState PointerState { get; }
        bool PointerGrabbed { get; set; }
        bool PointerActivated { get; }
        event EventHandler<PointerButtonEventArgs> PointerDown;
        event EventHandler<PointerButtonEventArgs> PointerUp;
        event EventHandler<PointerMoveEventArgs> PointerMove;
    }
}