using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    /// <summary>
    ///     状态Pong包
    /// </summary>
    public class StatePongPacket : Packet
    {
        public override int PacketId => 0x01;
        public override PacketOrigin Origin => PacketOrigin.Server;
        public override ProtocolState State => ProtocolState.Status;

        /// <summary>
        ///     负载（一般为Unix时间）
        /// </summary>
        public long Payload { get; set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            Payload = content.Read<Long>();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write((Long) Payload);
        }
    }
}