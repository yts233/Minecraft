using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public interface IViewTransformProvider : IMatrixCalculator<Matrix4, Vector4>
    {
        ICamera Camera { get; }
    }
}