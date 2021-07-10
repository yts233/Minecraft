using System;

namespace Minecraft.Input
{
    public interface IPointerContainer
    {
        IPointerState PointerState { get; }
        bool PointerGrabbed { get; set; }
        event Action<PointerButtonEventArgs> PointerDown;
        event Action<PointerButtonEventArgs> PointerUp;
        event Action<PointerMoveEventArgs> PointerMove;
    }
}