using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     符号型整数
    /// </summary>
    public struct Int : IDataType<int>
    {
        private int _value;

        private Int(int value)
        {
            _value = value;
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            var result = 0;
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

        int IDataType<int>.Value => _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator Int(int value)
        {
            return new Int(value);
        }

        public static implicit operator int(Int value)
        {
            return value._value;
        }
    }
}