#if false
namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class UpdateLightPacket : Packet
    {
        // TODO: edit class
        public int PacketId => 0x25;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Play;

        public void _ReadFromStream(IPacketEncoder content)
        {

        }

        public void _WriteToStream(IPacketEncoder content)
        {

        }
    }
}
#endif