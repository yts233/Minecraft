using System;
using System.IO;
using Kermalis.EndianBinaryIO;
using Minecraft.Data.Nbt;
using OpenTK.Mathematics;

namespace Minecraft.Protocol.Packets
{
    public class DefaultPacketCodec : IPacketCodec
    {
        private readonly Stream _baseStream;
        private readonly EndianBinaryReader _binaryReader;
        private readonly EndianBinaryWriter _binaryWriter;
        private readonly NbtReader _nbtReader;
        private readonly NbtWriter _nbtWriter;

        public IPacketCodec Clone(Stream baseStream)
        {
            return new DefaultPacketCodec(baseStream);
        }

        public DefaultPacketCodec()
        {
        }

        public DefaultPacketCodec(Stream baseStream)
        {
            _baseStream = baseStream;
            if (baseStream.CanRead)
            {
                _nbtReader = new NbtReader(baseStream);
                _binaryReader = new EndianBinaryReader(baseStream, Endianness.BigEndian, EncodingType.UTF8, BooleanSize.U8);
            }
            if (baseStream.CanWrite)
            {
                _nbtWriter = new NbtWriter(baseStream);
                _binaryWriter = new EndianBinaryWriter(baseStream, Endianness.BigEndian, EncodingType.UTF8, BooleanSize.U8);
            }
        }

        public Stream BaseStream => _baseStream;

        public float ReadAngle()
        {
            return _binaryReader.ReadSByte() / 128 * 180;
        }

        public bool ReadBoolean()
        {
            return _binaryReader.ReadBoolean();
        }

        public byte ReadByte()
        {
            return _binaryReader.ReadByte();
        }

        public double ReadDouble()
        {
            return _binaryReader.ReadDouble();
        }

        public short ReadInt16()
        {
            return _binaryReader.ReadInt16();
        }

        public int ReadInt32()
        {
            return _binaryReader.ReadInt32();
        }

        public long ReadInt64()
        {
            return _binaryReader.ReadInt64();
        }

        public NbtTag ReadNbtTag()
        {
            return _nbtReader.ReadTag();
        }

        public Vector3i ReadPosition()
        {
            var val = _binaryReader.ReadUInt64();
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
            return new Vector3i { X = x, Y = y, Z = z };
        }

        public sbyte ReadSByte()
        {
            return _binaryReader.ReadSByte();
        }

        public float ReadSingle()
        {
            return _binaryReader.ReadSingle();
        }

        public string ReadString()
        {
            return _binaryReader.ReadString(ReadVarInt(), false);
        }

        public ushort ReadUInt16()
        {
            return _binaryReader.ReadUInt16();
        }

        public Uuid ReadUuid()
        {
            return new Uuid(_binaryReader.ReadBytes(16));
        }

        public int ReadVarInt()
        {
            int value = 0;
            int bits = 0;
            byte currentByte;

            do
            {
                currentByte = _binaryReader.ReadByte();
                value |= (currentByte & 0x7F) << bits;

                bits += 7;
                if (bits > 35)
                {
                    throw new InvalidDataException("VarInt is too big");
                }
            }
            while ((currentByte & 0x80) == 0x80);
            return value;
        }

        public long ReadVarLong()
        {
            long value = 0;
            int bits = 0;
            byte currentByte;

            do
            {
                currentByte = _binaryReader.ReadByte();
                value |= (long)(currentByte & 0x7F) << bits;

                bits += 7;
                if (bits > 70)
                {
                    throw new InvalidDataException("VarLong is too big");
                }
            }
            while ((currentByte & 0x80) == 0x80);
            return value;
        }

        public void WriteAngle(float value)
        {
            _binaryWriter.Write((sbyte)(value / 180 * 128));
        }

        public void Write(bool value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(byte value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(double value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(NamedIdentifier value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(short value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(int value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(long value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(NbtTag value)
        {
            _nbtWriter.WriteTag(value);
        }

        public void WritePosition(Vector3i value)
        {
            var x = (ulong)value.X;
            var y = (ulong)value.Y;
            var z = (ulong)value.Z;
            var val = (x & 0x3FFFFFF) << 38 | (z & 0x3FFFFFF) << 12 | y & 0xFFF;
            _binaryWriter.Write(val);
        }

        public void Write(sbyte value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(float value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(string value)
        {
            WriteVarInt(value.Length);
            _binaryWriter.Write(value, false);
        }

        public void Write(ushort value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(Uuid value)
        {
            _binaryWriter.Write(value.ToByteArray());
        }

        public void WriteVarInt(int value)
        {
            while (true)
            {
                if ((value & ~0x7F) == 0)
                {
                    _binaryWriter.Write((byte)value);
                    return;
                }

                _binaryWriter.Write((byte)(value & 0x7F | 0x80));

                value >>= 7;
            }
        }

        public void WriteVarLong(long value)
        {
            while (true)
            {
                if ((value & ~0x7F) == 0)
                {
                    _binaryWriter.Write((byte)value);
                    return;
                }

                _binaryWriter.Write((byte)(value & 0x7F | 0x80));

                value >>= 7;
            }
        }
    }
}
