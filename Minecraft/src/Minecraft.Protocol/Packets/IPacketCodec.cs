using System.IO;
using System.Collections.Generic;
using System.Text;
using Minecraft.Data.Nbt;
using Minecraft.Numerics;
using System;

namespace Minecraft.Protocol.Packets
{
    public interface IPacketCodec
    {
        Stream BaseStream { get; }

        IPacketCodec Clone(Stream baseStream);

        int Skip(int byteCount)
        {
            if (byteCount <= 0)
                return 0;
            var buffer = new byte[8192];
            var skipCount = 0;
            while (byteCount > 0)
            {
                var s = BaseStream.Read(buffer, 0, Math.Min(8192, byteCount));
                if (s == 0) throw new EndOfStreamException();
                skipCount += s;
                byteCount -= s;
            }
            return skipCount;
        }

        int CopyTo(Stream destination, int byteCount)
        {
            if (byteCount <= 0)
                return 0;
            var buffer = new byte[8192];
            var copyCount = 0;
            while (byteCount > 0)
            {
                var s = BaseStream.Read(buffer, 0, Math.Min(8192, byteCount));
                if (s == 0) throw new EndOfStreamException();
                destination.Write(buffer, 0, s);
                byteCount -= s;
                copyCount += s;
            }
            return copyCount;
        }

        int CopyTo(Stream destination)
        {
            var buffer = new byte[8192];
            var copyCount = 0;
            int s;
            while ((s = BaseStream.Read(buffer, 0, 8192)) != 0)
            {
                destination.Write(buffer, 0, s);
                copyCount += s;
            }
            return copyCount;
        }

        bool ReadBoolean();
        sbyte ReadSByte();
        byte ReadByte();
        short ReadInt16();
        ushort ReadUInt16();
        int ReadInt32();
        long ReadInt64();
        float ReadSingle();
        double ReadDouble();
        string ReadString();
        NamedIdentifier ReadIdentifier()
        {
            return ReadString();
        }
        int ReadVarInt();
        long ReadVarLong();
        NbtTag ReadNbtTag();
        TNbtTag ReadNbtTag<TNbtTag>() where TNbtTag : NbtTag
        {
            return (TNbtTag)ReadNbtTag();
        }
        Vector3i ReadPosition();
        float ReadAngle();
        Uuid ReadUuid();
        Vector3f ReadVector3f()
        {
            var x = ReadSingle();
            var y = ReadSingle();
            var z = ReadSingle();
            return (x, y, z);
        }
        Rotation ReadRotation()
        {
            var x = ReadSingle();
            var y = ReadSingle();
            return (x, y);
        }
        Rotation ReadAngleRotation()
        {
            return (ReadAngle(), ReadAngle());
        }
        Vector3d ReadVector3d()
        {
            var x = ReadDouble();
            var y = ReadDouble();
            var z = ReadDouble();
            return (x, y, z);
        }
        TEnum ReadEnum<TEnum>() where TEnum : struct, Enum
        {
            Type typeFromHandle = typeof(TEnum);
            object value;
            switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeFromHandle)))
            {
                case TypeCode.Byte:
                    value = ReadByte();
                    break;
                case TypeCode.SByte:
                    value = ReadSByte();
                    break;
                case TypeCode.Int16:
                    value = ReadInt16();
                    break;
                case TypeCode.UInt16:
                    value = ReadUInt16();
                    break;
                case TypeCode.Int32:
                    value = ReadInt32();
                    break;
                case TypeCode.Int64:
                    value = ReadInt64();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("underlyingType");
            }

            return (TEnum)Enum.ToObject(typeFromHandle, value);
        }
        TEnum ReadVarIntEnum<TEnum>() where TEnum : struct, Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), ReadVarInt());
        }
        TEnum ReadVarLongEnum<TEnum>() where TEnum : struct, Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), ReadVarLong());
        }

        bool[] ReadBooleans(int count)
        {
            var array = new bool[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadBoolean();
            }
            return array;
        }
        sbyte[] ReadSBytes(int count)
        {
            var array = new sbyte[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadSByte();
            }
            return array;
        }
        short[] ReadInt16s(int count)
        {
            var array = new short[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadByte();
            }
            return array;
        }
        ushort[] ReadUInt16s(int count)
        {
            var array = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadUInt16();
            }
            return array;
        }
        int[] ReadInt32s(int count)
        {
            var array = new int[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadInt32();
            }
            return array;
        }
        long[] ReadInt64s(int count)
        {
            var array = new long[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadInt32();
            }
            return array;
        }
        float[] ReadSingles(int count)
        {
            var array = new float[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadSingle();
            }
            return array;
        }
        double[] ReadDoubles(int count)
        {
            var array = new double[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadSingle();
            }
            return array;
        }
        int[] ReadVarInts(int count)
        {
            var array = new int[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadVarInt();
            }
            return array;
        }
        long[] ReadVarLongs(int count)
        {
            var array = new long[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadVarLong();
            }
            return array;
        }
        NamedIdentifier[] ReadIdentifiers(int count)
        {
            var array = new NamedIdentifier[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadIdentifier();
            }
            return array;
        }

        void Write(bool value);
        void Write(sbyte value);
        void Write(byte value);
        void Write(short value);
        void Write(ushort value);
        void Write(int value);
        void Write(long value);
        void Write(float value);
        void Write(double value);
        void Write(string value);
        void Write(NamedIdentifier value)
        {
            Write((string)value);
        }
        void WriteVarInt(int value);
        void WriteVarLong(long value);
        void Write(NbtTag value);
        void WritePosition(Vector3i value);
        void WriteAngle(float value);
        void Write(Uuid value);
        void Write(Vector3f value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }
        void Write(Vector3d value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }
        void Write(Rotation value)
        {
            Write(value.Yaw);
            Write(value.Pitch);
        }
        void WriteAngleRotation(Rotation value)
        {
            WriteAngle(value.Yaw);
            WriteAngle(value.Pitch);
        }
        void Write<TEnum>(TEnum value) where TEnum : Enum
        {
            switch (Type.GetTypeCode(Enum.GetUnderlyingType(value.GetType())))
            {
                case TypeCode.Byte:
                    Write(Convert.ToByte(value));
                    break;
                case TypeCode.SByte:
                    Write(Convert.ToSByte(value));
                    break;
                case TypeCode.Int16:
                    Write(Convert.ToInt16(value));
                    break;
                case TypeCode.UInt16:
                    Write(Convert.ToUInt16(value));
                    break;
                case TypeCode.Int32:
                    Write(Convert.ToInt32(value));
                    break;
                case TypeCode.Int64:
                    Write(Convert.ToInt64(value));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("underlyingType");
            }
        }
        void WriteVarIntEnum<TEnum>(TEnum value) where TEnum : Enum
        {
            WriteVarInt(Convert.ToInt32(value));
        }
        void WriteVarLongEnum<TEnum>(TEnum value) where TEnum : Enum
        {
            WriteVarLong(Convert.ToInt64(value));
        }

        void Write(bool[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        void Write(sbyte[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        void Write(short[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        void Write(ushort[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        void Write(int[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        void Write(long[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        void Write(float[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        void Write(double[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        void WriteVarInts(int[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                WriteVarInt(value[i]);
            }
        }
        void WriteVarLongs(long[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                WriteVarLong(value[i]);
            }
        }
        void Write(NamedIdentifier[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }

    }
}
