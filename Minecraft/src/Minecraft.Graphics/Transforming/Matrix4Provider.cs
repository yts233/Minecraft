using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class Matrix4Provider : IMatrixProvider<Matrix4, Vector4>
    {
        public Matrix4 Matrix { get; set; }
        public Matrix4 GetMatrix() => Matrix;
        public Vector4 Transform(Vector4 vector) => Matrix * vector;
    }
}