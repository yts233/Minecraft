using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public interface IProjectionTransformProvider : IMatrixCalculator<Matrix4, Vector4>
    {
    }
}