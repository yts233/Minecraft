using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Client
{
    /// <summary>
    /// 状态Ping包
    /// </summary>
    public class StatusPingPacket : Packet
    {
        public override int PacketId => 0x01;
        public override PacketBoundTo BoundTo => PacketBoundTo.Server;
        public override ProtocolState State => ProtocolState.Status;

        /// <summary>
        /// 负载（一般为Unix时间）
        /// </summary>
        public long Payload { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Payload = content.ReadInt64();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Payload);
        }
    }
}