using OpenTK.Mathematics;

namespace Minecraft.Input
{
    public interface IAxisInput
    {
        AxisRange Range { get; }
        Vector3 Value { get; }

        void Update();
    }
}