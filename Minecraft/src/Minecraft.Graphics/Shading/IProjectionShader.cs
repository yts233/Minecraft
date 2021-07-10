using OpenTK.Mathematics;

namespace Minecraft.Graphics.Shading
{
    public interface IProjectionShader : IShader
    {
        Matrix4 Projection { get; set; }
    }
}