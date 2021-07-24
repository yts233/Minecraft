using System.IO;
using System.Text;

namespace Minecraft.Text
{
    /// <summary>
    ///     UTF-8写入器
    /// </summary>
    public class Utf8Writer : TextWriter
    {
        private readonly Stream _baseStream;

        /// <summary>
        ///     创建<see cref="Utf8Writer" />
        /// </summary>
        /// <param name="stream">流</param>
        public Utf8Writer(Stream stream)
        {
            _baseStream = stream;
        }

        public override Encoding Encoding { get; } = Encoding.UTF8;

        public override void Write(char value)
        {
            if (value > 0x07ff)
            {
                _baseStream.WriteByte((byte)((value >> 12) | 0b11100000));
                _baseStream.WriteByte((byte)(((value >> 6) & 0b00111111) | 0b10000000));
                _baseStream.WriteByte((byte)((value & 0b00111111) | 0b10000000));
            }
            else if (value > 0x007f)
            {
                _baseStream.WriteByte((byte) ((value >> 6) | 0b11000000));
                _baseStream.WriteByte((byte) ((value & 0b00111111) | 0b10000000));
            }
            else if (value <= 0x007f)
            {
                _baseStream.WriteByte((byte)value);
            }
            else
            {
                throw new InvalidDataException("the valid range of char is 0x0000~0xffff");
            }
        }
    }
}