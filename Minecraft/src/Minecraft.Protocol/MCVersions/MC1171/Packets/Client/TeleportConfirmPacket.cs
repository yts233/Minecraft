using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class TeleportConfirmPacket : IPacket
    {
        public int PacketId => 0x00;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The teleport id.
        /// </summary>
        /// <remarks>The ID given by the <see cref="PlayerPositionAndLookPacket"/> packet.</remarks>
        public int TeleportId { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            TeleportId = content.ReadVarInt();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.WriteVarInt(TeleportId);
        }
    }
}
