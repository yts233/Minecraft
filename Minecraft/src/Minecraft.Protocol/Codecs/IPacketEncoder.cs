using System.IO;
using System.Collections.Generic;
using System.Text;
using Minecraft.Data.Nbt;
using Minecraft.Numerics;
using System;

namespace Minecraft.Protocol.Codecs
{
    public interface IPacketCodec
    {
        public Stream BaseStream { get; }

        public bool ReadBoolean();
        public sbyte ReadSByte();
        public byte ReadByte();
        public short ReadInt16();
        public ushort ReadUInt16();
        public int ReadInt32();
        public long ReadInt64();
        public float ReadSingle();
        public double ReadDouble();
        public string ReadString();
        public NamedIdentifier ReadIdentifier()
        {
            return ReadString();
        }
        public int ReadVarInt();
        public long ReadVarLong();
        public NbtTag ReadNbtTag();
        public TNbtTag ReadNbtTag<TNbtTag>() where TNbtTag : NbtTag
        {
            return (NbtTag)ReadNbtTag();
        }
        public Vector3i ReadPosition();
        public float ReadAngle();
        public Uuid ReadUuid();
        public Vector3f ReadVector3f()
        {
            var x = ReadSingle();
            var y = ReadSingle();
            var z = ReadSingle();
            return (x, y, z);
        }
        public Rotation ReadRotation()
        {
            var x = ReadSingle();
            var y = ReadSingle();
            return (x, y);
        }
        public Rotation ReadAngleRotation()
        {
            return (ReadAngle(), ReadAngle());
        }
        public Vector3d ReadVector3d()
        {
            var x = ReadDouble();
            var y = ReadDouble();
            var z = ReadDouble();
            return (x, y, z);
        }
        public TEnum ReadEnum<TEnum>() where TEnum : struct, Enum
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
        public TEnum ReadVarIntEnum<TEnum>() where TEnum : struct, Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), ReadVarInt());
        }
        public TEnum ReadVarLongEnum<TEnum>() where TEnum : struct, Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), ReadVarLong());
        }

        public bool[] ReadBooleans(int count)
        {
            var array = new bool[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadBoolean();
            }
            return array;
        }
        public sbyte[] ReadSBytes(int count)
        {
            var array = new sbyte[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadSByte();
            }
            return array;
        }
        public short[] ReadInt16s(int count)
        {
            var array = new short[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadByte();
            }
            return array;
        }
        public ushort[] ReadUInt16s(int count)
        {
            var array = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadUInt16();
            }
            return array;
        }
        public int[] ReadInt32s(int count)
        {
            var array = new int[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadInt32();
            }
            return array;
        }
        public long[] ReadInt64s(int count)
        {
            var array = new long[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadInt32();
            }
            return array;
        }
        public float[] ReadSingles(int count)
        {
            var array = new float[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadSingle();
            }
            return array;
        }
        public double[] ReadDoubles(int count)
        {
            var array = new double[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadSingle();
            }
            return array;
        }
        public int[] ReadVarInts(int count)
        {
            var array = new int[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadVarInt();
            }
            return array;
        }
        public long[] ReadVarLongs(int count)
        {
            var array = new long[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadVarLong();
            }
            return array;
        }
        public NamedIdentifier[] ReadIdentifiers(int count)
        {
            var array = new NamedIdentifier[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadIdentifier();
            }
            return array;
        }

        public void Write(bool value);
        public void Write(sbyte value);
        public void Write(byte value);
        public void Write(short value);
        public void Write(ushort value);
        public void Write(int value);
        public void Write(long value);
        public void Write(float value);
        public void Write(double value);
        public void Write(string value);
        public void Write(NamedIdentifier value)
        {
            Write((string)value);
        }
        public void WriteVarInt(int value);
        public void WriteVarLong(long value);
        public void Write(NbtTag value);
        public void WritePosition(Vector3i value);
        public void WriteAngle(float value);
        public void Write(Uuid value);
        public void Write(Vector3f value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }
        public void Write(Vector3d value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }
        public void Write(Rotation value)
        {
            Write(value.Yaw);
            Write(value.Pitch);
        }
        public void WriteAngleRotation(Rotation value)
        {
            WriteAngle(value.Yaw);
            WriteAngle(value.Pitch);
        }
        public void Write<TEnum>(TEnum value) where TEnum : Enum
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
        public void WriteVarIntEnum<TEnum>(TEnum value) where TEnum : Enum
        {
            WriteVarInt(Convert.ToInt32(value));
        }
        public void WriteVarLongEnum<TEnum>(TEnum value) where TEnum : Enum
        {
            WriteVarLong(Convert.ToInt64(value));
        }

        public void Write(bool[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        public void Write(sbyte[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        public void Write(short[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        public void Write(ushort[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        public void Write(int[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        public void Write(long[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        public void Write(float[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        public void Write(double[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }
        public void WriteVarInts(int[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                WriteVarInt(value[i]);
            }
        }
        public void WriteVarLongs(long[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                WriteVarLong(value[i]);
            }
        }
        public void Write(NamedIdentifier[] value)
        {
            int count = value.Length;
            for (int i = 0; i < count; i++)
            {
                Write(value[i]);
            }
        }

    }
}
