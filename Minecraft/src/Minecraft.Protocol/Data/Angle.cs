using System.IO;

namespace Minecraft.Protocol.Data
{
    public struct Angle : IDataType<sbyte>, IDataType<float>
    {
        private sbyte _value;

        private Angle(sbyte value)
        {
            _value = value;
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var read = this.ReadByte(stream);
            _value = (sbyte)read;
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            stream.WriteByte((byte)_value);
        }

        sbyte IDataType<sbyte>.Value => _value;
        float IDataType<float>.Value => this;

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator Angle(float value)
        {
            return new Angle((sbyte)(value / 180 * 128));
        }

        public static implicit operator float(Angle value)
        {
            return (float)value._value / 128 * 180;
        }

        public static explicit operator Angle(sbyte value)
        {
            return new Angle(value);
        }

        public static explicit operator sbyte(Angle value)
        {
            return value._value;
        }
    }
}
