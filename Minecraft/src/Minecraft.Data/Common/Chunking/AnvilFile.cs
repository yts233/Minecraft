using System;
using System.Collections.Generic;
using System.IO;

namespace Minecraft.Data.Common.Chunking
{
    /// <summary>
    /// Anvil file Data
    /// </summary>
    public class AnvilFile
    {
        private const int MaxDataLength = 1073750016;
        private readonly MemoryStream _stream;

        public AnvilFile(byte[] data)
        {
            if (data.Length > MaxDataLength) throw new ArgumentOutOfRangeException(nameof(data), "data is too long");
            _stream = new MemoryStream(data);
        }

        // public AnvilFile(Stream stream)
        // {
        //     _stream = new MemoryStream();
        //     var buffer = new byte[1048576];
        //     int s;
        //     while ((s = stream.Read(buffer, 0, 1048576)) != 0)
        //     {
        //         _stream.Write(buffer, 0, s);
        //         if (_stream.Length > MaxDataLength) throw new ArgumentOutOfRangeException(nameof(stream), "data is too long");
        //     }
        // }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>offset and length per 4KB</returns>
        /// <exception cref="IndexOutOfRangeException">index is less than zero or great than 1024</exception>
        public (int offset,int length) GetChunkLocation(int index)
        {
            if (index < 0 || index >= 1024)
                throw new IndexOutOfRangeException();
            throw new NotImplementedException();
        }
    }
}