using OpenTK.Graphics.OpenGL;

namespace Minecraft.Graphics.Texturing
{
    public static class Extensions
    {
        /// <summary>
        /// 纹理替换
        /// </summary>
        /// <param name="texture">纹理</param>
        /// <param name="image">图像</param>
        /// <param name="xOffset">x轴偏移量</param>
        /// <param name="yOffset">y轴偏移量</param>
        /// <returns></returns>
        public static ITexture SubImage(this ITexture texture, Image image, int xOffset, int yOffset)
        {
            GL.TextureSubImage2D(texture.Handle,
                0,
                xOffset, yOffset,
                image.Width, image.Height,
                PixelFormat.Rgba, PixelType.UnsignedByte,
                image.Data);
            return texture;
        }

        /// <summary>
        /// 纹理替换
        /// </summary>
        /// <param name="texture">纹理</param>
        /// <param name="data">图像数据</param>
        /// <param name="xOffset">x轴偏移量</param>
        /// <param name="yOffset">y轴偏移量</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <returns></returns>
        public static ITexture SubImage(this ITexture texture, byte[] data, int xOffset, int yOffset, int width,
            int height)
        {
            GL.TextureSubImage2D(texture.Handle,
                0,
                xOffset, yOffset,
                width, height,
                PixelFormat.Rgba, PixelType.UnsignedByte,
                data);
            return texture;
        }

        /// <summary>
        /// 生成纹理Mipmap
        /// </summary>
        /// <remarks>请确保纹理已被绑定</remarks>
        /// <param name="texture">纹理</param>
        /// <returns></returns>
        public static ITexture GenerateMipmaps(this ITexture texture)
        {
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            return texture;
        }
    }
}