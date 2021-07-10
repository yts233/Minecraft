using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     符号型长整数
    /// </summary>
    public struct Long : IDataType<long>
    {
        private long _value;

        private Long(long value)
        {
            _value = value;
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            var result = 0;
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
                stream.WriteByte((byte) (value >> 24));
                value <<= 8;
            }
        }

        long IDataType<long>.Value => _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator Long(long value)
        {
            return new Long(value);
        }

        public static implicit operator long(Long value)
        {
            return value._value;
        }
    }
}