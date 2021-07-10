using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     数据类型
    /// </summary>
    public interface IDataType<out T> : IDataType
    {
        /// <summary>
        ///     获取对应的值
        /// </summary>
        /// <value>对应的值</value>
        T Value { get; }
    }

    public interface IDataType
    {
        /// <summary>
        ///     从流内读取
        /// </summary>
        /// <param name="stream">Stream.</param>
        void ReadFromStream(Stream stream);

        /// <summary>
        ///     写入到流
        /// </summary>
        /// <param name="stream">Stream.</param>
        void WriteToStream(Stream stream);
    }
}