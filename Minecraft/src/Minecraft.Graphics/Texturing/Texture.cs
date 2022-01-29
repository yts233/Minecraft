using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Minecraft.Graphics.Texturing
{
    public class Texture : ITexture
    {
        public static readonly ITexture Null = new Texture(0, TextureTarget.Texture2D);
        private readonly int _handle;
        private readonly TextureTarget _target;

        public Texture(int handle, TextureTarget target)
        {
            _handle = handle;
            _target = target;
        }

        int IHandle.Handle => _handle;

        public void Bind()
        {
            GL.BindTexture(_target, _handle);
        }
    }
}