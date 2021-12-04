using Minecraft.Protocol.Codecs;
#if false
namespace Minecraft.Protocol.Packets.Server
{
    public class UpdateLightPacket : Packet
    {
        // TODO: edit class
        public override int PacketId => 0x25;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        protected override void _ReadFromStream(IPacketEncoder content)
        {

        }

        protected override void _WriteToStream(IPacketEncoder content)
        {

        }
    }
}
#endif