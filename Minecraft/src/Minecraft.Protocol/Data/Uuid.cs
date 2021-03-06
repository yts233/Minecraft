﻿using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     通用唯一标识符
    /// </summary>
    public struct Uuid : IDataType<Minecraft.Uuid>
    {
        Minecraft.Uuid IDataType<Minecraft.Uuid>.Value => _value;
        private Minecraft.Uuid _value;

        public Uuid(Minecraft.Uuid uuid)
        {
            _value = uuid;
        }

        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var buffer = new byte[16];
            if (stream.Read(buffer, 0, 16) != 16) throw new EndOfStreamException();
            _value = new Minecraft.Uuid(buffer);
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            var buffer = _value.ToByteArray();
            stream.Write(buffer, 0, 16);
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static explicit operator Minecraft.Uuid(Uuid value)
        {
            return value._value;
        }

        public static explicit operator Uuid(Minecraft.Uuid value)
        {
            return new Uuid(value);
        }
    }
}