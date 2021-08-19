using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    /// 无符号型整数
    /// </summary>
    public struct UnsignedInt : IDataType<uint>
    {
        private uint _value;

        private UnsignedInt(uint value)
        {
            _value = value;
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            uint result = 0;
            for (var i = 0; i < 4; i++)
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
            for (var i = 0; i < 4; i++)
            {
                stream.WriteByte((byte) (value >> 24));
                value <<= 8;
            }
        }

        uint IDataType<uint>.Value => _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator UnsignedInt(uint value)
        {
            return new UnsignedInt(value);
        }

        public static implicit operator uint(UnsignedInt value)
        {
            return value._value;
        }
    }
}