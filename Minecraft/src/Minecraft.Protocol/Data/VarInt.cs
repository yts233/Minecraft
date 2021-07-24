using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     可变长度整数
    /// </summary>
    public struct VarInt : IDataType<int>
    {
        private VarInt(int value)
        {
            _value = value;
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var result = 0;
            var bitOffset = 0;
            byte read;
            do
            {
                read = this.ReadByte(stream);
                result |= (read & 0b01111111) << bitOffset;
                if (bitOffset == 35) throw new InvalidDataException("Invalid data");
                bitOffset += 7;
            } while ((read & 0b10000000) != 0);

            _value = result;
        }

        int IDataType<int>.Value => _value;
        private int _value;

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            var value = _value;
            do
            {
                var temp = (byte) (value & 0b01111111);
                value >>= 7;
                if (value != 0) temp |= 0b10000000;

                stream.WriteByte(temp);
            } while (value != 0);
        }

        public static implicit operator VarInt(int value)
        {
            return new VarInt(value);
        }

        public static implicit operator int(VarInt value)
        {
            return value._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}