using Kermalis.EndianBinaryIO;
using System.IO;

namespace Minecraft.Protocol.Data
{
    public struct Double : IDataType<double>
    {
        private double _value;

        private Double(double value)
        {
            _value = value;
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var buffer = new byte[8];
            if (stream.Read(buffer, 0, 8) < 8)
                throw new EndOfStreamException();
            _value = EndianBitConverter.BytesToDouble(buffer, 0, Endianness.BigEndian);
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            var buffer = EndianBitConverter.DoubleToBytes(_value, Endianness.BigEndian);
            stream.Write(buffer, 0, 8);
        }

        double IDataType<double>.Value => _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator Double(double value)
        {
            return new Double(value);
        }

        public static implicit operator double(Double value)
        {
            return value._value;
        }
    }
}
