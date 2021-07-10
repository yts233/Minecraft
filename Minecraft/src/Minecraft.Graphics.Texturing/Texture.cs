using Minecraft.Graphics.Rendering;

namespace Minecraft.Graphics.Texturing
{
    public class Texture : ITexture
    {
        public static readonly ITexture Null = new Texture(0);
        private readonly int _handle;

        public Texture(int handle)
        {
            _handle = handle;
        }

        int IHandle.Handle => _handle;
    }
}