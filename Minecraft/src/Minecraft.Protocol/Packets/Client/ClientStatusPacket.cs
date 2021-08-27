using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Client
{
    public class ClientStatusPacket : Packet
    {
        public override int PacketId => 0x04;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        public ClientStatusAction ActionId { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            ActionId = (ClientStatusAction)content.ReadVarInt();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.WriteVarInt((int)ActionId);
        }
    }
}
