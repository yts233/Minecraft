#define FixEndOfStream
using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Server
{
    public class UpdateViewPositionPacket : Packet
    {
        public override int PacketId => 0x49;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        public int ChunkX { get; set; }

        public int ChunkZ { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            ChunkX = content.ReadVarInt();
            ChunkZ = content.ReadVarInt();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.WriteVarInt(ChunkX);
            content.WriteVarInt(ChunkZ);
        }
    }
}
