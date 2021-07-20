using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     无符号型字节
    /// </summary>
    public struct UnsignedByte : IDataType<byte>
    {
        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var read = this.ReadByte(stream);
            _value = read;
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            stream.WriteByte(  _value);
        }

        private UnsignedByte(byte value)
        {
            _value = value;
        }

        private byte _value;
        byte IDataType<byte>.Value => _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator byte(UnsignedByte value)
        {
            return value._value;
        }

        public static implicit operator UnsignedByte(byte value)
        {
            return new UnsignedByte(value);
        }
    }
}