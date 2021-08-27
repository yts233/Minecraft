#define FixEndOfStream
using Minecraft.Numerics;
using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    public class SpawnPlayerPacket : Packet
    {
        public override int PacketId => 0x04;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// Player's EID.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Player's UUID.
        /// </summary>
        /// <remarks>When in online mode, the UUIDs must be valid and have valid skin blobs.</remarks>
        public Uuid PlayerUuid { get; set; }

        public Vector3d Position { get; set; }

        public Rotation Rotation { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            EntityId = content.ReadVarInt();
            PlayerUuid = content.ReadUuid();
            Position = content.ReadVector3d();
            Rotation = content.ReadAngleRotation();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.WriteVarInt(EntityId)
                .Write(PlayerUuid)
                .Write(Position)
                .WriteAngleRotation(Rotation);
        }
    }
}
