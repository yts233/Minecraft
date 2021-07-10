using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public interface ITransformable
    {
        Vector4 Transform(Vector4 vector);
    }
}