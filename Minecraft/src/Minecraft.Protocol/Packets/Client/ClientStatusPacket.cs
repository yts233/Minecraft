using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Client
{
    public class ClientStatusPacket : Packet
    {
        public override int PacketId => 0x04;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        public ClientStatusAction ActionId { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            ActionId = content.ReadVarIntEnum<ClientStatusAction>();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.WriteVarIntEnum(ActionId);
        }
    }
}
