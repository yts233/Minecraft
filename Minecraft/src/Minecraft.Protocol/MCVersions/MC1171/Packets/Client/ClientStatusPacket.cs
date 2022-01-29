using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class ClientStatusPacket : IPacket
    {
        public int PacketId => 0x04;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        public ClientStatusAction ActionId { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            ActionId = content.ReadVarIntEnum<ClientStatusAction>();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.WriteVarIntEnum(ActionId);
        }
    }
}
