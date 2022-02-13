using OpenTK.Mathematics;

namespace Minecraft.Input
{
    public interface IAxisInput : IUpdatable
    {
        AxisRange Range { get; }
        Vector3 Value { get; }
    }
}