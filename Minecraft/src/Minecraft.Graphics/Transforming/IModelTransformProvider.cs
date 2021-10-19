using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public interface IModelTransformProvider : IMatrixCalculator<Matrix4, Vector4>
    {
        IModel Model { get; set; }
    }
}