using System;
using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     可变长度整数枚举
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct VarIntEnum<T> : IEnum<T> where T : Enum
    {
        private T _value;

        private VarIntEnum(T value)
        {
            _value = value;
        }

        private enum IntEnum
        {
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var content = this.GetContent(stream);
            _value = (T) (Enum) (IntEnum) (int) content.Read<VarInt>();
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            var content = this.GetContent(stream);
            content.Write((VarInt) (int) (IntEnum) (Enum) _value);
        }

        T IDataType<T>.Value => _value;

        public static implicit operator T(VarIntEnum<T> varIntEnum)
        {
            return varIntEnum._value;
        }

        public static implicit operator VarIntEnum<T>(T value)
        {
            return new VarIntEnum<T>(value);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}