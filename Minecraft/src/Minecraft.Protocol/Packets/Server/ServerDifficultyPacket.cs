using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    public class ServerDifficultyPacket : Packet
    {
        public override int PacketId => 0x0E;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        public Difficulty Difficulty { get; set; }
        public bool DifficultyLocked { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            Difficulty = (Difficulty)content.ReadUnsignedByte();
            DifficultyLocked = content.ReadBoolean();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.Write((byte)Difficulty)
                .Write(DifficultyLocked);
        }
    }
}
