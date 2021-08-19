using Minecraft.Protocol.Data;

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

        protected override void _ReadFromStream(ByteArray content)
        {
            Payload = content.ReadLong();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write(Payload);
        }
    }
}