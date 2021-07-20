using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class Matrix3Provider : IMatrixProvider<Matrix3, Vector3>
    {
        public Matrix3 Matrix { get; set; }
        public Matrix3 GetMatrix() => Matrix;
        public Vector3 Transform(Vector3 vector) => Matrix * vector;
    }
}