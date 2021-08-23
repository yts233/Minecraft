using System;
using Minecraft.Protocol.Data;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;

namespace Minecraft.Protocol.Packets
{
    /// <summary>
    /// 数据包的传输介质
    /// </summary>
    public class DataPacket : Packet, IDisposable
    {
        private int _packetId;

        internal DataPacket(PacketBoundTo origin, ProtocolState state = ProtocolState.Any)
        {
            BoundTo = origin;
            State = state;
        }

        internal DataPacket(int packetId, PacketBoundTo origin, ByteArray content,
            ProtocolState state = ProtocolState.Any)
        {
            _packetId = packetId;
            BoundTo = origin;
            State = state;
            Content = content;
        }

        /// <summary>
        /// 数据包的内容
        /// </summary>
        public ByteArray Content { get; private set; }

        public override int PacketId => _packetId;
        public override PacketBoundTo BoundTo { get; }
        public override ProtocolState State { get; }

        /// <summary>
        /// 释放内容
        /// </summary>
        public void Dispose()
        {
            Content?.Dispose();
        }

        protected override void _ReadFromStream(ByteArray content)
        {
            _packetId = content.ReadVarInt();
            Content = content.Read<ByteArray>();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            using var buffer = new ByteArray(0);
            Content.Position = 0;
            buffer.WriteVarInt(_packetId).Write(Content);
            buffer.Position = 0;
            content.WriteVarInt((int)buffer.Length).Write(buffer);
        }

        public void WriteCompressedToStream(Stream stream, int threshold)
        {
            var content = this.GetContent(stream); // get upload stream
            using var buffer = new ByteArray(0); // uncompressed packetId and data
            Content.Position = 0;
            buffer.WriteVarInt(_packetId).Write(Content);
            buffer.Position = 0;
            var dataLength = (int)buffer.Length;
            using var buffer4 = new ByteArray(0);
            if (dataLength >= threshold)
            {
                using var buffer2 = new ByteArray(0); // compressed packetId and data
                using var compressedStream = new DeflaterOutputStream(buffer2);
                this.GetContent(compressedStream).Write(buffer);
                compressedStream.Flush();
                buffer2.Position = 0;
                buffer4.WriteVarInt(dataLength).Write(buffer2);
            }
            else
            {
                buffer4.WriteVarInt(0).Write(buffer);
            }
            buffer4.Position = 0;
            content.WriteVarInt((int)buffer4.Length).Write(buffer4);
        }
    }
}