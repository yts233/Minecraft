using OpenTK.Mathematics;

namespace Minecraft.Graphics.Shading
{
    public interface IViewShader : IShader
    {
        Matrix4 View { get; set; }
    }
}