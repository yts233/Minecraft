using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Client
{
    public class TeleportConfirmPacket : Packet
    {
        public override int PacketId => 0x00;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The teleport id.
        /// </summary>
        /// <remarks>The ID given by the <see cref="PlayerPositionAndLookPacket"/> packet.</remarks>
        public int TeleportId { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            TeleportId = content.ReadVarInt();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.WriteVarInt(TeleportId);
        }
    }
}
