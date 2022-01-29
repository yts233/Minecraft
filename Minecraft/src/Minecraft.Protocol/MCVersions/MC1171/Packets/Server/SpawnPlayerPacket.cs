#define FixEndOfStream
using Minecraft.Numerics;
using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class SpawnPlayerPacket : IPacket
    {
        public int PacketId => 0x04;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Play;

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

        public void ReadFromStream(IPacketCodec content)
        {
            EntityId = content.ReadVarInt();
            PlayerUuid = content.ReadUuid();
            Position = content.ReadVector3d();
            Rotation = content.ReadAngleRotation();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.WriteVarInt(EntityId);
            content.Write(PlayerUuid);
            content.Write(Position);
            content.WriteAngleRotation(Rotation);
        }
    }
}
