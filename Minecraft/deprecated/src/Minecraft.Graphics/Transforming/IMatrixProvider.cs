using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public interface IMatrixProvider<out TMatrix, TVector> : ITransformable<TVector>
        where TMatrix : struct
        where TVector : struct
    {
        TMatrix GetMatrix();
    }
}