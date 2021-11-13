using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    public class ChunkDataPacket : Packet
    {
        public override int PacketId => 0x22;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
        public bool[] PrimaryBitMask { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            
            throw new System.NotImplementedException();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            throw new System.NotImplementedException();
        }
    }
}
