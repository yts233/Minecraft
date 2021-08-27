using Minecraft.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Minecraft.Protocol.Data
{
    public struct Position : IDataType<Vector3i>
    {
        private Vector3i _value;

        public Vector3i Value => _value;

        private Position(Vector3i value)
        {
            _value = value;
        }

        public void ReadFromStream(Stream stream)
        {
            var val = this.GetContent(stream).ReadUnsignedLong();
            var x = (int)(val >> 38);
            var y = (int)(val & 0xFFF);
            var z = (int)(val << 26 >> 38);
            const int a = 2 << 25;
            const int b = 2 << 26;
            const int c = 2 << 11;
            const int d = 2 << 12;
            if (x >= a) x -= b;
            if (y >= c) y -= d;
            if (z >= a) z -= b;
            _value = new Vector3i { X = x, Y = y, Z = z };
        }

        public void WriteToStream(Stream stream)
        {
            var x = (ulong)_value.X;
            var y = (ulong)_value.Y;
            var z = (ulong)_value.Z;
            var val = ((x & 0x3FFFFFF) << 38) | ((z & 0x3FFFFFF) << 12) | (y & 0xFFF);
            this.GetContent(stream).Write(val);
        }

        public static implicit operator Position(Vector3i value)
        {
            return new Position(value);
        }

        public static implicit operator Vector3i(Position value)
        {
            return value._value;
        }
    }
}
