using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Client
{
    /// <summary>
    /// 状态请求包
    /// </summary>
    public class StatusRequestPacket : Packet
    {
        public override int PacketId => 0x00;
        public override PacketBoundTo BoundTo => PacketBoundTo.Server;
        public override ProtocolState State => ProtocolState.Status;

        protected override void ReadFromStream_(IPacketCodec content)
        {
            //empty
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            //empty
        }
    }
}