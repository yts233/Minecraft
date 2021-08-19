using System;
using System.IO;

namespace Minecraft.Text
{
    /// <summary>
    /// UTF-8读取器
    /// </summary>
    public class Utf8Reader : TextReader
    {
        private readonly Stream _baseStream;

        /// <summary>
        /// 创建<see cref="Utf8Reader" />
        /// </summary>
        /// <param name="stream">流</param>
        public Utf8Reader(Stream stream)
        {
            _baseStream = stream;
        }

        public override int Read()
        {
            var tmp = _baseStream.ReadByte();
            if (tmp == -1) return -1;

            var result = 0;
            var extByteCount = 0;
            if ((tmp & 0b10000000) == 0) return tmp & 0b01111111;

            if ((tmp & 0b11100000) == 0b11000000)
            {
                extByteCount = 1;
                result |= tmp & 0b00011111;
            }

            if ((tmp & 0b11110000) == 0b11100000)
            {
                extByteCount = 2;
                result |= tmp & 0b00001111;
            }

            for (var i = 0; i < extByteCount; i++)
            {
                result <<= 6;
                tmp = _baseStream.ReadByte();
                if (tmp == -1) return -1;
                if ((tmp & 0b11000000) != 0b10000000) return -1;
                result |= tmp & 0b00111111;
            }

            return result;
        }
    }
}