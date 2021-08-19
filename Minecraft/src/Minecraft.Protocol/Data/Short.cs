using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    /// 符号型短整数
    /// </summary>
    public struct Short : IDataType<short>
    {
        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            ushort result = 0;
            for (var i = 0; i < 2; i++)
            {
                var read = this.ReadByte(stream);
                result <<= 8;
                result |= read;
            }

            _value = (short) result;
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

        private short _value;
        short IDataType<short>.Value => _value;

        private Short(short value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator Short(short value)
        {
            return new Short(value);
        }

        public static implicit operator short(Short value)
        {
            return value._value;
        }
    }
}