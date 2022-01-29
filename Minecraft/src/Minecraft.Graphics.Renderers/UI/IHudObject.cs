using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.UI
{
    public interface IHudObject
    {
        Vector3 Position { get; set; }
        Color4 Color { get; set; }
    }
}