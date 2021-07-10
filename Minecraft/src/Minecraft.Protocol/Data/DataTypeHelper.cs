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
    }
}