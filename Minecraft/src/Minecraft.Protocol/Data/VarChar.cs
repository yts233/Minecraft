﻿using System;
using System.IO;
using Minecraft.Text;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     可变字符串
    /// </summary>
    public struct VarChar : IDataType<string>
    {
        private VarChar(string value)
        {
            if (value.Length > 32767)
                throw new ArgumentOutOfRangeException(nameof(value), "Length of the value cannot be larger than 32767");
            _value = value;
        }

        string IDataType<string>.Value => _value;
        private string _value;
        private int Length => _value?.Length ?? -1;

        void IDataType.ReadFromStream(Stream stream)
        {
            this.CheckStreamReadable(stream);
            var content = this.GetContent(stream);
            var length = content.Read<VarInt>();
            if (length > 32767) throw new InvalidDataException("String out of range!");
            var reader = new Utf8Reader(stream);
            var buffer = new char[length];
            var s = reader.ReadBlock(buffer, 0, length);
            _value = new string(buffer, 0, s);
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            var content = this.GetContent(stream);
            content.Write((VarInt) Length);
            new Utf8Writer(content).Write(_value);
        }

        public static implicit operator VarChar(string value)
        {
            return new VarChar(value);
        }

        public static implicit operator string(VarChar value)
        {
            return value._value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}