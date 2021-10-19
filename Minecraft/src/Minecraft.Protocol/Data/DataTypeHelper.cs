using Minecraft.Data.Nbt;
using Minecraft.Numerics;
using System;
using System.IO;
using System.Linq;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    /// 数据类型助手
    /// </summary>
    public static class DataTypeHelper
    {
        /// <summary>
        /// 检查流是否可读
        /// </summary>
        /// <param name="dataType">终为null</param>
        /// <param name="stream">要检查的流</param>
        public static void CheckStreamReadable(this IDataType dataType, Stream stream)
        {
            if (!stream.CanRead) throw new NotSupportedException("Stream cannot be read!");
        }

        /// <summary>
        /// 检查流是否可写
        /// </summary>
        /// <param name="dataType">终为null</param>
        /// <param name="stream">要检查的流</param>
        public static void CheckStreamWritable(this IDataType dataType, Stream stream)
        {
            if (!stream.CanWrite) throw new NotSupportedException("Stream cannot be written!");
        }

        /// <summary>
        /// 获取流的<see cref="ByteArray" />形式
        /// </summary>
        /// <param name="dataType">终为null</param>
        /// <param name="stream">流</param>
        /// <returns></returns>
        internal static ByteArray GetContent(this IDataType dataType, Stream stream)
        {
            return stream is ByteArray byteArray ? byteArray : new ByteArray(stream);
        }

        /// <summary>
        /// 从流内读取字节
        /// </summary>
        /// <param name="dataType">终为null</param>
        /// <param name="stream">流</param>
        /// <returns></returns>
        /// <exception cref="EndOfStreamException">在流尾读取</exception>
        public static byte ReadByte(this IDataType dataType, Stream stream)
        {
            var read = stream.ReadByte();
            if (read == -1)
                throw new EndOfStreamException("End of stream!");
            return (byte)read;
        }

        /// <summary>
        /// 从流内读取数据
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="stream">流</param>
        /// <typeparam name="TDataType">数据类型</typeparam>
        /// <returns></returns>
        public static IDataType<TDataType> ReadFromStream<TDataType>(this IDataType<TDataType> dataType, Stream stream)
        {
            dataType.ReadFromStream(stream);
            return dataType;
        }

        /// <summary>
        /// 将数据类型写入至流
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="stream">流</param>
        /// <typeparam name="TDataType">数据类型</typeparam>
        /// <returns></returns>
        public static IDataType<TDataType> WriteToStream<TDataType>(this IDataType<TDataType> dataType, Stream stream)
        {
            dataType.WriteToStream(stream);
            return dataType;
        }

        public static ByteArray Write(this ByteArray byteArray, sbyte value)
        {
            byteArray.Write((Byte)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, float value)
        {
            byteArray.Write((Float)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, double value)
        {
            byteArray.Write((Double)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, byte value)
        {
            byteArray.Write((UnsignedByte)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, bool value)
        {
            byteArray.Write((Boolean)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, byte[] value)
        {
            byteArray.Write(new ByteArray(value));
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, int value)
        {
            byteArray.Write((Int)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, long value)
        {
            byteArray.Write((Long)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, short value)
        {
            byteArray.Write((Short)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, uint value)
        {
            byteArray.Write((UnsignedInt)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, ushort value)
        {
            byteArray.Write((UnsignedShort)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, Minecraft.Uuid value)
        {
            byteArray.Write((Uuid)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, string value)
        {
            byteArray.Write((String)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, ulong value)
        {
            byteArray.Write((UnsignedLong)value);
            return byteArray;
        }

        public static ByteArray WriteVarInt(this ByteArray byteArray, int value)
        {
            byteArray.Write((VarInt)value);
            return byteArray;
        }

        private enum EmptyEnum
        {
        }

        public static ByteArray WriteArray<T>(this ByteArray byteArray, T[] array) where T : IDataType, new()
        {
            var array1 = new Array<T>(array);
            array1.WriteToStream(byteArray);
            return byteArray;
        }

        public static ByteArray WriteVarIntEnum(this ByteArray byteArray, Enum value)
        {
            byteArray.Write((VarInt)(int)(EmptyEnum)value);
            return byteArray;
        }

        public static bool ReadBoolean(this ByteArray byteArray)
        {
            return byteArray.Read<Boolean>();
        }

        public static int ReadInt(this ByteArray byteArray)
        {
            return byteArray.Read<Int>();
        }

        public static long ReadLong(this ByteArray byteArray)
        {
            return byteArray.Read<Long>();
        }

        public static float ReadFloat(this ByteArray byteArray)
        {
            return byteArray.Read<Float>();
        }

        public static double ReadDouble(this ByteArray byteArray)
        {
            return byteArray.Read<Double>();
        }

        public static short ReadShort(this ByteArray byteArray)
        {
            return byteArray.Read<Short>();
        }

        public static byte ReadUnsignedByte(this ByteArray byteArray)
        {
            return byteArray.Read<UnsignedByte>();
        }

        public static uint ReadUnsignedInt(this ByteArray byteArray)
        {
            return byteArray.Read<UnsignedInt>();
        }

        public static ulong ReadUnsignedLong(this ByteArray byteArray)
        {
            return byteArray.Read<UnsignedLong>();
        }

        public static ushort ReadUnsignedShort(this ByteArray byteArray)
        {
            return byteArray.Read<UnsignedShort>();
        }

        public static Minecraft.Uuid ReadUuid(this ByteArray byteArray)
        {
            return (Minecraft.Uuid)byteArray.Read<Uuid>();
        }

        public static string ReadString(this ByteArray byteArray)
        {
            return byteArray.Read<String>();
        }

        public static int ReadVarInt(this ByteArray byteArray)
        {
            return byteArray.Read<VarInt>();
        }

        public static float ReadAngle(this ByteArray byteArray)
        {
            return byteArray.Read<Angle>();
        }

        public static ByteArray WriteAngle(this ByteArray byteArray, float value)
        {
            byteArray.Write((Angle)value);
            return byteArray;
        }

        public static Vector3d ReadVector3d(this ByteArray byteArray)
        {
            return new Vector3d { X = byteArray.ReadDouble(), Y = byteArray.ReadDouble(), Z = byteArray.ReadDouble() };
        }

        public static Vector3f ReadVector3f(this ByteArray byteArray)
        {
            return new Vector3f { X = byteArray.ReadFloat(), Y = byteArray.ReadFloat(), Z = byteArray.ReadFloat() };
        }

        public static Vector3i ReadVector3i(this ByteArray byteArray)
        {
            return byteArray.Read<Position>();
        }

        public static Rotation ReadRotation(this ByteArray byteArray)
        {
            return new Rotation { Yaw = byteArray.ReadFloat(), Pitch = byteArray.ReadFloat() };
        }

        public static Rotation ReadAngleRotation(this ByteArray byteArray)
        {
            return new Rotation { Yaw = byteArray.ReadAngle(), Pitch = byteArray.ReadAngle() };
        }

        public static ByteArray Write(this ByteArray byteArray, Vector3d value)
        {
            byteArray.Write(value.X);
            byteArray.Write(value.Y);
            byteArray.Write(value.Z);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, Vector3f value)
        {
            byteArray.Write(value.X);
            byteArray.Write(value.Y);
            byteArray.Write(value.Z);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, Vector3i value)
        {
            byteArray.Write((Position)value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, Rotation value)
        {
            byteArray.Write(value.Yaw);
            byteArray.Write(value.Pitch);
            return byteArray;
        }

        public static ByteArray WriteAngleRotation(this ByteArray byteArray, Rotation value)
        {
            byteArray.WriteAngle(value.Yaw);
            byteArray.WriteAngle(value.Pitch);
            return byteArray;
        }

        public static T[] ReadArray<T>(this ByteArray byteArray, int length) where T : IDataType, new()
        {
            var array = new Array<T>
            {
                Length = length
            };
            array.ReadFromStream(byteArray);
            return array.Value;
        }

        public static TValueType[] ReadArray<TDataType, TValueType>(this ByteArray byteArray, int length) where TDataType : IDataType<TValueType>, new()
        {
            var array = new Array<TDataType>
            {
                Length = length
            };
            array.ReadFromStream(byteArray);
            return array.Value.Select(ele => ele.Value).ToArray();
        }

        public static T ReadVarIntEnum<T>(this ByteArray byteArray) where T : Enum
        {
            return byteArray.Read<VarIntEnum<T>>();
        }

        public static NbtTag ReadNbt(this ByteArray byteArray)
        {
            using var reader = new NbtReader(byteArray);
            return reader.ReadTag();
        }

        public static T ReadNbt<T>(this ByteArray byteArray) where T : NbtTag
        {
            return (T)byteArray.ReadNbt();
        }

        public static ByteArray WriteNbt(this ByteArray byteArray, NbtTag tag)
        {
            using var writer = new NbtWriter(byteArray);
            writer.WriteTag(tag);
            return byteArray;
        }
    }
}