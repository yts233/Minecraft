using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Client
{
    /// <summary>
    ///     状态请求包
    /// </summary>
    public class StateRequestPacket : Packet
    {
        public override int PacketId => 0x00;
        public override PacketOrigin Origin => PacketOrigin.Server;
        public override ProtocolState State => ProtocolState.Status;

        protected override void _ReadFromStream(ByteArray content)
        {
        }

        protected override void _WriteToStream(ByteArray content)
        {
        }
    }
}