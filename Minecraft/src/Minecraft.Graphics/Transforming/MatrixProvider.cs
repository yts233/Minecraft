using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class MatrixProvider : IMatrixProvider
    {
        public Matrix4 Matrix { get; set; }

        public Matrix4 GetMatrix()
        {
            return Matrix;
        }
    }
}