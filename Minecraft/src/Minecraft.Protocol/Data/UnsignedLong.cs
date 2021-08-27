using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    /// 无符号型长整数
    /// </summary>
    public struct UnsignedLong : IDataType<ulong>
    {
        private ulong _value;

        private UnsignedLong(ulong value)
        {
            _value = value;
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            var result = 0UL;
            for (var i = 0; i < 8; i++)
            {
                var read = this.ReadByte(stream);
                result <<= 8;
                result |= read;
            }

            _value = result;
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            var value = _value;
            for (var i = 0; i < 8; i++)
            {
                stream.WriteByte((byte)(value >> 56));
                value <<= 8;
            }
        }

        ulong IDataType<ulong>.Value => _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator UnsignedLong(ulong value)
        {
            return new UnsignedLong(value);
        }

        public static implicit operator ulong(UnsignedLong value)
        {
            return value._value;
        }
    }
}