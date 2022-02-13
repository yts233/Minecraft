using OpenTK.Mathematics;
using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class EntityTeleportPacket : IPacket
    {
        public int PacketId => 0x61;
        public PacketBoundTo BoundTo => PacketBoundTo.Client;
        public ProtocolState State => ProtocolState.Play;

        public int EntityId { get; set; }

        public Vector3d Position { get; set; }

        public Vector2 Rotation { get; set; }

        /// <summary>
        /// 实体是否在地面上
        /// </summary>
        public bool OnGround { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            EntityId = content.ReadVarInt();
            Position = content.ReadVector3d();
            Rotation = content.ReadAngleRotation();
            OnGround = content.ReadBoolean();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.WriteVarInt(EntityId);
            content.Write(Position);
            content.WriteAngleRotation(Rotation);
            content.Write(OnGround);
        }
    }
}