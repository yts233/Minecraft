using System;
using Minecraft.Graphics.Rendering;
using Minecraft.Input;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Windowing
{
    public interface IRenderWindowContainer : IRenderContainer, IPointerContainer, IKeyboardContainer, IGameTickContainer, IDisposable
    {
        Vector2i Location { get; }
        Vector2i Size { get; set; }
        bool IsFullScreen { get; set; }
    }
}