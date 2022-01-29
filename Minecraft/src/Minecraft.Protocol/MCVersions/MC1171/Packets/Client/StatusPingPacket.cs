using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    /// <summary>
    /// 状态Ping包
    /// </summary>
    public class StatusPingPacket : IPacket
    {
        public int PacketId => 0x01;
        public PacketBoundTo BoundTo => PacketBoundTo.Server;
        public ProtocolState State => ProtocolState.Status;

        /// <summary>
        /// 负载（一般为Unix时间）
        /// </summary>
        public long Payload { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Payload = content.ReadInt64();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Payload);
        }
    }
}