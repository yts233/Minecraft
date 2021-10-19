using OpenTK.Mathematics;

namespace Minecraft.Input
{
    public interface IExternAxisInput : IAxisInput
    {
        IAxisInput BaseInput { get; }
    }
}