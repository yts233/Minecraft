using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class PlayerMovementPacket : IPacket
    {
        public int PacketId => 0x14;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        public bool OnGround { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            OnGround = content.ReadBoolean();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(OnGround);
        }
    }
}
