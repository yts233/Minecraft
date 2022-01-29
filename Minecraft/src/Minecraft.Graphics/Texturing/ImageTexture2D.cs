using System.IO;
using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Minecraft.Graphics.Texturing
{
    public class ImageTexture2D : ITexture,ITexture2D
    {
        private readonly int _handle;

        public ImageTexture2D(Stream stream) : this(new Image(stream))
        {
        }

        public ImageTexture2D(Image image)
        {
            Width = image.Width;
            Height = image.Height;
            _handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.NearestMipmapNearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public int Width { get; }

        public int Height { get; }

        int IHandle.Handle => _handle;

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }
    }
}