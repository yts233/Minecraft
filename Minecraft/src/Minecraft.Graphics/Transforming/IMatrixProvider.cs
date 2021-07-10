using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public interface IMatrixProvider : ITransformable
    {
        Vector4 ITransformable.Transform(Vector4 vector)
        {
            return vector * GetMatrix();
        }

        Matrix4 GetMatrix();
    }
}