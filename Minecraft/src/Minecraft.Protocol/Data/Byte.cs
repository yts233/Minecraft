using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     符号型字节
    /// </summary>
    public struct Byte : IDataType<sbyte>
    {
        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var read = this.ReadByte(stream);
            _value = (sbyte)read;
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            stream.WriteByte((byte) _value);
        }

        private Byte(sbyte value)
        {
            _value = value;
        }

        private sbyte _value;
        sbyte IDataType<sbyte>.Value => _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator sbyte(Byte value)
        {
            return value._value;
        }

        public static implicit operator Byte(sbyte value)
        {
            return new Byte(value);
        }
    }
}