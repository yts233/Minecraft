using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Minecraft.Protocol.Packets
{
    /// <summary>
    /// 数据包
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// 包ID
        /// </summary>
        int PacketId { get; }

        /// <summary>
        /// 包绑定至
        /// </summary>
        PacketBoundTo BoundTo { get; }

        /// <summary>
        /// 协议状态
        /// </summary>
        ProtocolState State { get; }

        /// <summary>
        /// 从流读入
        /// </summary>
        /// <param name="content">流</param>
        void ReadFromStream(IPacketCodec content);

        /// <summary>
        /// 将此数据包写入到流
        /// </summary>
        /// <param name="content">流</param>
        void WriteToStream(IPacketCodec content);

        /// <summary>
        /// 检验值
        /// </summary>
        void VerifyValues()
        {
        }
    }
}