using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public interface ITransformable<TVector> where TVector : struct
    {
        TVector Transform(TVector vector);
    }
}