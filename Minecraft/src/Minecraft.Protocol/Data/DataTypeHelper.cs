using System;
using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     数据类型助手
    /// </summary>
    public static class DataTypeHelper
    {
        /// <summary>
        ///     检查流是否可读
        /// </summary>
        /// <param name="dataType">终为null</param>
        /// <param name="stream">要检查的流</param>
        public static void CheckStreamReadable(this IDataType dataType, Stream stream)
        {
            if (!stream.CanRead) throw new NotSupportedException("Stream cannot be read!");
        }

        /// <summary>
        ///     检查流是否可写
        /// </summary>
        /// <param name="dataType">终为null</param>
        /// <param name="stream">要检查的流</param>
        public static void CheckStreamWritable(this IDataType dataType, Stream stream)
        {
            if (!stream.CanWrite) throw new NotSupportedException("Stream cannot be written!");
        }

        /// <summary>
        ///     获取流的<see cref="ByteArray" />形式
        /// </summary>
        /// <param name="dataType">终为null</param>
        /// <param name="stream">流</param>
        /// <returns></returns>
        internal static ByteArray GetContent(this IDataType dataType, Stream stream)
        {
            return stream is ByteArray byteArray ? byteArray : new ByteArray(stream);
        }

        /// <summary>
        ///     从流内读取字节
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
            return (byte) read;
        }

        /// <summary>
        ///     从流内读取数据
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
        ///     将数据类型写入至流
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
            byteArray.Write((Byte) value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, byte value)
        {
            byteArray.Write((UnsignedByte) value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, bool value)
        {
            byteArray.Write((Boolean) value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, byte[] value)
        {
            byteArray.Write(new ByteArray(value));
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, int value)
        {
            byteArray.Write((Int) value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, long value)
        {
            byteArray.Write((Long) value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, short value)
        {
            byteArray.Write((Short) value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, uint value)
        {
            byteArray.Write((UnsignedInt) value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, ushort value)
        {
            byteArray.Write((UnsignedShort) value);
            return byteArray;
        }

        public static ByteArray Write(this ByteArray byteArray, Minecraft.Uuid value)
        {
            byteArray.Write((Uuid) value);
            return byteArray;
        }

        public static ByteArray WriteVar(this ByteArray byteArray, string value)
        {
            byteArray.Write((VarChar) value);
            return byteArray;
        }

        public static ByteArray WriteVar(this ByteArray byteArray, int value)
        {
            byteArray.Write((VarInt) value);
            return byteArray;
        }

        private enum EmptyEnum
        {
        }

        public static ByteArray WriteVar(this ByteArray byteArray, Enum value)
        {
            byteArray.Write((VarInt) (int) (EmptyEnum) value);
            return byteArray;
        }

        public static bool ReadBoolean(this ByteArray byteArray)
        {
            return byteArray.Read<Boolean>();
        }

        public static sbyte ReadByte(this ByteArray byteArray)
        {
            return byteArray.Read<Byte>();
        }

        public static int ReadInt(this ByteArray byteArray)
        {
            return byteArray.Read<Int>();
        }

        public static long ReadLong(this ByteArray byteArray)
        {
            return byteArray.Read<Long>();
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

        public static ushort ReadUnsignedShort(this ByteArray byteArray)
        {
            return byteArray.Read<UnsignedShort>();
        }

        public static Minecraft.Uuid ReadUuid(this ByteArray byteArray)
        {
            return (Minecraft.Uuid) byteArray.Read<Uuid>();
        }

        public static string ReadVarChar(this ByteArray byteArray)
        {
            return byteArray.Read<VarChar>();
        }

        public static int ReadVarInt(this ByteArray byteArray)
        {
            return byteArray.Read<VarInt>();
        }

        public static T ReadVarIntEnum<T>(this ByteArray byteArray) where T : Enum
        {
            return byteArray.Read<VarIntEnum<T>>();
        }
    }
}