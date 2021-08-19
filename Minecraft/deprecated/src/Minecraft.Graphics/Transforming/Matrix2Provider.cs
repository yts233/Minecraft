using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class Matrix2Provider : IMatrixProvider<Matrix2, Vector2>
    {
        public Matrix2 Matrix { get; set; }
        public Matrix2 GetMatrix() => Matrix;
        public Vector2 Transform(Vector2 vector) => Matrix * vector;
    }
}