using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class ChunkDataPacket : IPacket
    {
        public int PacketId => 0x22;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Play;

        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
        public bool[] PrimaryBitMask { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            
            //throw new System.NotImplementedException();
        }

        public void WriteToStream(IPacketCodec content)
        {
            //throw new System.NotImplementedException();
        }
    }
}
