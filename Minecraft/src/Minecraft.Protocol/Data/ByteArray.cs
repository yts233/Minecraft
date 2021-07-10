using System;
using System.IO;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    ///     字节序列
    /// </summary>
    public class ByteArray : Stream, IDataType<Stream>
    {
        private Stream _stream;

        /// <summary>
        ///     创建一个空的字节序列，除非读取，无法写入数据
        /// </summary>
        public ByteArray()
        {
        }

        /// <summary>
        ///     从字节数组创建字节序列并共享内存
        /// </summary>
        /// <param name="buffer"></param>
        public ByteArray(byte[] buffer) : this(buffer, 0, buffer.Length)
        {
        }

        /// <summary>
        ///     从字节数组创建字节序列并共享内存
        /// </summary>
        /// <param name="buffer">缓冲</param>
        /// <param name="index">索引</param>
        /// <param name="count">数量</param>
        public ByteArray(byte[] buffer, int index, int count)
        {
            _stream = new MemoryStream(buffer, index, count);
        }

        /// <summary>
        ///     从流创建字节序列
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="copyFromStream">是否从流复制。</param>
        public ByteArray(Stream stream, bool copyFromStream = false)
        {
            if (copyFromStream)
                this.ReadFromStream(stream);
            else _stream = stream;
        }

        /// <summary>
        ///     从流内复制定长的字节序列作为内容
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="length">长度</param>
        public ByteArray(Stream stream, int length)
        {
            CopyFromStream(stream, length);
        }

        /// <summary>
        ///     创建一个空的可写字节序列
        /// </summary>
        /// <param name="capacity">字节序列的大小。为0则无限大</param>
        public ByteArray(int capacity)
        {
            if (capacity == 0) _stream = new MemoryStream();
            else if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be negative!");
            else _stream = new MemoryStream(capacity);
        }

        public override bool CanRead => _stream?.CanRead ?? true;

        public override bool CanSeek => _stream?.CanSeek ?? true;

        public override bool CanWrite => _stream?.CanWrite ?? true;

        public override long Length => _stream.Length;

        public override long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        Stream IDataType<Stream>.Value => this;

        void IDataType.ReadFromStream(Stream stream)
        {
            CopyFromStream(stream);
        }

        void IDataType.WriteToStream(Stream stream)
        {
            this.CheckStreamWritable(stream);
            var buffer = new byte[8192];
            int s;
            while ((s = _stream.Read(buffer, 0, 8192)) != 0)
                stream.Write(buffer, 0, s);
        }

        private void CopyFromStream(Stream stream, int length = -1)
        {
            this.CheckStreamReadable(stream);
            _stream = new MemoryStream();
            var buffer = new byte[8192];
            var readToEnd = length == -1;
            while (length > 0 || readToEnd)
            {
                var s = stream.Read(buffer, 0, readToEnd ? 8192 : Math.Min(length, 8192));
                if (s == 0) break;
                _stream.Write(buffer, 0, s);
                length -= s;
            }

            _stream.Position = 0;
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        /// <summary>
        ///     写入某个特定的数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public ByteArray Write(IDataType data)
        {
            data.WriteToStream(this);
            return this;
        }

        /// <summary>
        ///     读取某个特定的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns></returns>
        public T Read<T>() where T : IDataType, new()
        {
            var tmp = new T();
            tmp.ReadFromStream(this);
            return tmp;
        }

        /// <summary>
        ///     获取流
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            return _stream;
        }

        public override string ToString()
        {
            return $"length: {Length}";
        }
    }
}