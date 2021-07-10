using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL;

namespace Minecraft.Graphics.Texturing
{
    public class EmptyTexture : ITexture
    {
        private readonly int _handle;

        public EmptyTexture(int width, int height)
        {
            _handle = GL.GenTexture();
            var image = new Image(width, height);
            image.InitializeEmptyImage();
            GL.BindTexture(TextureTarget.Texture2D, _handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.NearestMipmapNearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        int IHandle.Handle => _handle;
    }
}