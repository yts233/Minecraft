using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class ServerDifficultyPacket : IPacket
    {
        public int PacketId => 0x0E;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Play;

        public Difficulty Difficulty { get; set; }
        public bool DifficultyLocked { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Difficulty = content.ReadEnum<Difficulty>();
            DifficultyLocked = content.ReadBoolean();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Difficulty);
            content.Write(DifficultyLocked);
        }
    }
}
