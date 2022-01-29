using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class UpdateViewPositionPacket : IPacket
    {
        public int PacketId => 0x49;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Play;

        public int ChunkX { get; set; }

        public int ChunkZ { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            ChunkX = content.ReadVarInt();
            ChunkZ = content.ReadVarInt();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.WriteVarInt(ChunkX);
            content.WriteVarInt(ChunkZ);
        }
    }
}
