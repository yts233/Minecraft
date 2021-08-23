using Minecraft.Protocol.Data;
#if false
namespace Minecraft.Protocol.Packets.Server
{
    public class UpdateLightPacket : Packet
    {
        // TODO: edit class
        public override int PacketId => 0x25;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        protected override void _ReadFromStream(ByteArray content)
        {

        }

        protected override void _WriteToStream(ByteArray content)
        {

        }
    }
}
#endif