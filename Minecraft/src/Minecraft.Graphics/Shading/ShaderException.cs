namespace Minecraft.Graphics.Shading
{
    /// <summary>
    /// 因着色器而引发的异常
    /// </summary>
    public class ShaderException : GraphicException
    {
        public ShaderException(string message) : base(message)
        {
        }
    }
}