using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Server
{
    public class HeldItemChangePacket : Packet
    {
        public override int PacketId => 0x48;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        public HotbarSlot Slot { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Slot = content.ReadEnum<HotbarSlot>();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Slot);
        }
    }
}
