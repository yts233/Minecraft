using Kermalis.EndianBinaryIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Minecraft.Protocol.Data
{
    public struct Float : IDataType<float>
    {
        private float _value;

        private Float(float value)
        {
            _value = value;
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var buffer = new byte[4];
            if (stream.Read(buffer, 0, 4) < 4)
                throw new EndOfStreamException();
            _value = EndianBitConverter.BytesToSingle(buffer, 0, Endianness.BigEndian);
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            var buffer = EndianBitConverter.SingleToBytes(_value, Endianness.BigEndian);
            stream.Write(buffer, 0, 4);
        }

        float IDataType<float>.Value => _value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator Float(float value)
        {
            return new Float(value);
        }

        public static implicit operator float(Float value)
        {
            return value._value;
        }
    }
}
