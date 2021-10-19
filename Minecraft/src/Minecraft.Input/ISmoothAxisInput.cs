using OpenTK.Mathematics;

namespace Minecraft.Input
{
    public interface ISmoothAxisInput : IExternAxisInput
    {
        float Speed { get; set; }
    }
}