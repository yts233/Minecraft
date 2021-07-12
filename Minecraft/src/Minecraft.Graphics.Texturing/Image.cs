using System;
using System.IO;
using Minecraft.Extensions;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Minecraft.Graphics.Texturing
{
    public readonly ref struct Image
    {
        private static void CheckSize(int width, int height)
        {
            if (width != height || height % width != 0)
                throw new TextureException("test: (height == width || height % width == 0)");
            switch (width)
            {
                case 8:
                case 16:
                case 18:
                case 32:
                case 64:
                case 128:
                case 256:
                case 512:
                case 1024:
                    break;
                default:
                    throw new TextureException("valid width: 8, 16, 18, 32, 64, 128, 256, 512, 1024");
            }
        }

        public Image(byte[] data, int width, int height, bool force = false)
        {
            if (!force)
                CheckSize(width, height);
            Data = data;
            Width = width;
            Height = height;
            FrameCount = height / width;
            FrameSize = (width * width) << 2;
        }

        public Image(int width, int height, bool force = false)
        {
            if (!force)
                CheckSize(width, height);
            /*
             * data struct
             * [Location] [Color]
             * Location:
             *     [Y::height.GetBitsCount()] [X::width.GetBitsCount()]
             * Color:
             *     [RGBA::2]
             */
            var buffer = new byte[(width * height) << 2];
            Data = buffer;
            Width = width;
            Height = height;
            FrameCount = height / width;
            FrameSize = (width * width) << 2;
        }

        public Image(Stream stream, bool force = false) : this(SixLabors.ImageSharp.Image.Load<Rgba32>(stream), force)
        {
        }

        public Image(SixLabors.ImageSharp.Image<Rgba32> image, bool force = false)
        {
            //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left.
            image.Mutate(context => context.Flip(FlipMode.Vertical));
            image.TryGetSinglePixelSpan(out var span);
            var pixels = new byte[(image.Width * image.Height) << 2];
            for (var i = 0; i < span.Length; i++)
            {
                var rgba32 = span[i];
                var tmp1 = i << 2;
                pixels[tmp1] = rgba32.R;
                pixels[tmp1 | 0b01] = rgba32.G;
                pixels[tmp1 | 0b10] = rgba32.B;
                pixels[tmp1 | 0b11] = rgba32.A;
            }

            Data = pixels;
            Width = image.Width;
            Height = image.Height;
            FrameCount = image.Height / image.Width;
            FrameSize = (image.Width * image.Width) << 2;
        }

        public void InitializeEmptyImage()
        {
            var buffer = Data;
            var length = Data.Length;
            var p = 0b1111 << (Width.GetBitsCount() + 1);
            for (var i = 0; i < length; i++)
            {
                var q = i & 0b11;
                if ((i & 0b111100) == 0b111100 || (i & p) == p)
                {
                    //#6b3f7f
                    buffer[i] = q switch
                    {
                        0b00 => 0x6B,
                        0b01 => 0x3F,
                        0b10 => 0x7F,
                        0b11 => 0xFF,
                        _ => 0x00
                    };
                }
                else
                {
                    //#d67fff
                    buffer[i] = q switch
                    {
                        0b00 => 0xD6,
                        0b01 => 0x7F,
                        0b10 => 0xFF,
                        0b11 => 0xFF,
                        _ => 0x00
                    };
                }
            }
        }

        public Image GetFrame(int index)
        {
            var buffer = new byte[FrameSize];
            Array.Copy(Data, FrameSize * index, buffer, 0, FrameSize);
            return new Image(buffer, Width, Width);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public byte[] Data { get; }
        public int Width { get; }
        public int Height { get; }
        public int FrameCount { get; }

        /// <summary>
        /// size of frame
        /// </summary>
        /// <remarks>in bytes</remarks>
        public int FrameSize { get; }

        public ref struct Enumerator
        {
            private readonly Image _image;
            private int _index;

            internal Enumerator(Image image)
            {
                _image = image;
                _index = -1;
            }

            public bool MoveNext()
            {
                var num = this._index + 1;
                if (num >= _image.FrameCount)
                    return false;
                _index = num;
                return true;
            }

            public Image Current => _image.GetFrame(_index);
        }
    }
}