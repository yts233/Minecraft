using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    public class ChuckDataPacket : Packet
    {
        // TODO: edit class
        public override int PacketId => 0x22;

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
