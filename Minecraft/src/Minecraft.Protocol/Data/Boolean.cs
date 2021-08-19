using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    /// 布尔值数据类型
    /// </summary>
    public struct Boolean : IDataType<bool>
    {
        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var read = this.ReadByte(stream);
            _value = read == 0x01;
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            stream.WriteByte(_value ? (byte) 0x01 : (byte) 0x00);
        }

        private bool _value;
        bool IDataType<bool>.Value => _value;

        private Boolean(bool value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator bool(Boolean value)
        {
            return value._value;
        }

        public static implicit operator Boolean(bool value)
        {
            return new Boolean(value);
        }
    }
}