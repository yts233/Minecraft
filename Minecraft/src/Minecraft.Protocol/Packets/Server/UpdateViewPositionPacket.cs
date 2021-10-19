#define FixEndOfStream
using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    public class UpdateViewPositionPacket : Packet
    {
        public override int PacketId => 0x49;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        public int ChunkX { get; set; }

        public int ChunkZ { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            ChunkX = content.ReadVarInt();
            ChunkZ = content.ReadVarInt();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.WriteVarInt(ChunkX)
                .WriteVarInt(ChunkZ);
        }
    }
}
