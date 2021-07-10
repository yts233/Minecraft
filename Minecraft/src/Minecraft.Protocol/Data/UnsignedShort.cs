using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     无符号型短整数
    /// </summary>
    public struct UnsignedShort : IDataType<ushort>
    {
        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            ushort result = 0;
            for (var i = 0; i < 2; i++)
            {
                var read = this.ReadByte(stream);
                result |= (ushort) (read << (8 * i));
            }

            _value = result;
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            var value = _value;
            for (var i = 0; i < 2; i++)
            {
                stream.WriteByte((byte) (value >> 8));
                value <<= 8;
            }
        }

        private ushort _value;
        ushort IDataType<ushort>.Value => _value;

        private UnsignedShort(ushort value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator UnsignedShort(ushort value)
        {
            return new UnsignedShort(value);
        }

        public static implicit operator ushort(UnsignedShort value)
        {
            return value._value;
        }
    }
}