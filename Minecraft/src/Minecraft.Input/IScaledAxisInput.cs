using OpenTK.Mathematics;

namespace Minecraft.Input
{
    public interface IScaledAxisInput : IExternAxisInput
    {
        float Scale { get; set; }
        Vector3 IAxisInput.Value => BaseInput.Value * Scale;
    }
}