using OpenTK.Mathematics;

namespace Minecraft.Graphics.Shading
{
    public interface IModelShader : IShader
    {
        Matrix4 Model { get; set; }
    }
}