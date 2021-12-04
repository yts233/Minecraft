using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Server
{
    /// <summary>
    /// 状态Pong包
    /// </summary>
    public class StatusPongPacket : Packet
    {
        public override int PacketId => 0x01;
        public override PacketBoundTo BoundTo => PacketBoundTo.Client;
        public override ProtocolState State => ProtocolState.Status;

        /// <summary>
        /// 负载（一般为Unix时间）
        /// </summary>
        public long Payload { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Payload = content.ReadLong();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Payload);
        }
    }
}