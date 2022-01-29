using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class HeldItemChangePacket : IPacket
    {
        public int PacketId => 0x48;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Play;

        public HotbarSlot Slot { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Slot = content.ReadEnum<HotbarSlot>();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Slot);
        }
    }
}
