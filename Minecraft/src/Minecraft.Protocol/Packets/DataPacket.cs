using System;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;

namespace Minecraft.Protocol.Packets
{
    /// <summary>
    /// 数据包的传输介质
    /// </summary>
    public class DataPacket : IPacket, IDisposable
    {
        public DataPacket(IPacketCodec defaultCodec)
        {
            BaseStream = new MemoryStream();
            Content = defaultCodec.Clone(BaseStream);
        }

        public MemoryStream BaseStream { get; private set; }
        public IPacketCodec Content { get; }
        public int PacketId { get; set; }
        public int PacketLength { get; set; }
        public int DataLength { get; set; }
        public PacketBoundTo BoundTo { get; set; }
        public ProtocolState State { get; set; }

        public void Dispose()
        {
            BaseStream.Dispose();
        }

        public void ResetPosition()
        {
            BaseStream.Position = 0;
        }

        public void ReadFromStream(IPacketCodec rawCodec, PacketBoundTo boundTo, Func<int> compressThresholdCallback, Func<ProtocolState> stateCallback)
        {
            BoundTo = boundTo;
            var length = rawCodec.ReadVarInt(); // Length of Packet ID + Data
            State = stateCallback();
            PacketLength = length;
            var threshold = compressThresholdCallback();
            int dataLength;
            using var recvStream = new MemoryStream(); //no extra bytes read
            rawCodec.CopyTo(recvStream, length);
            var recvCodec = rawCodec.Clone(recvStream);
            recvStream.Position = 0;
            if (threshold == 0 || (dataLength = recvCodec.ReadVarInt()) < threshold)
            {
                DataLength = length;
                recvCodec.CopyTo(BaseStream, length);
                BaseStream.Position = 0;
                PacketId = Content.ReadVarInt();
                return;
            }
            DataLength = dataLength;
            using var compressedStream = new InflaterInputStream(recvStream) { IsStreamOwner = false };
            rawCodec.Clone(compressedStream).CopyTo(BaseStream, dataLength);
            compressedStream.Dispose();
            BaseStream.Position = 0;
            PacketId = Content.ReadVarInt();
        }

        public void ReadFromPacket(IPacket packet)
        {
            PacketId = packet.PacketId;
            BoundTo = packet.BoundTo;
            State = packet.State;
            Content.WriteVarInt(packet.PacketId);
            var pos = BaseStream.Position;
            packet.WriteToStream(Content);
            BaseStream.Position = pos;
            PacketLength = (int)BaseStream.Length;
            DataLength = (int)BaseStream.Length;
        }

        public void WriteToStream(IPacketCodec rawCodec, Func<int> compressThresholdCallback)
        {
            var threshold = compressThresholdCallback();
            if (threshold == 0 || DataLength < threshold)
            {
                PacketLength = (int)BaseStream.Length;
                rawCodec.WriteVarInt(DataLength);
                Content.CopyTo(rawCodec.BaseStream, DataLength);
                return;
            }
            var stream = new MemoryStream();
            var content = Content.Clone(stream);
            content.WriteVarInt(DataLength);
            var compressedStream = new DeflaterOutputStream(stream);
            Content.CopyTo(compressedStream, DataLength);
            compressedStream.Flush();
            stream.Position = 0;
            PacketLength = (int)stream.Length;
            rawCodec.WriteVarInt(PacketLength);
            content.CopyTo(rawCodec.BaseStream, PacketLength);
            compressedStream.Dispose();
            stream.Dispose();
        }

        void IPacket.ReadFromStream(IPacketCodec content)
        {
            content.BaseStream.CopyTo(BaseStream);
        }
        void IPacket.WriteToStream(IPacketCodec content)
        {
            BaseStream.CopyTo(content.BaseStream);
        }
    }
}