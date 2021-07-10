using OpenTK.Mathematics;

namespace Minecraft.Graphics.Shading
{
    internal interface IViewShader : IShader
    {
        Matrix4 View { get; set; }
    }
}