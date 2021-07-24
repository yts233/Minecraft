using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    public class HeldItemChangePacket : Packet
    {
        public override int PacketId => 0x48;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        public HotbarSlot Slot { get; set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            Slot = (HotbarSlot)content.ReadByte();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write((sbyte)Slot);
        }
    }
}
