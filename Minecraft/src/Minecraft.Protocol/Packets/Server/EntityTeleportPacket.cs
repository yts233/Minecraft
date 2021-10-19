using Minecraft.Numerics;
using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    public class EntityTeleportPacket : Packet
    {
        public override int PacketId => 0x61;
        public override PacketBoundTo BoundTo => PacketBoundTo.Client;
        public override ProtocolState State => ProtocolState.Play;

        public int EntityId { get; set; }

        public Vector3d Position { get; set; }

        public Rotation Rotation { get; set; }

        /// <summary>
        /// 实体是否在地面上
        /// </summary>
        public bool OnGround { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            EntityId = content.ReadVarInt();
            Position = content.ReadVector3d();
            Rotation = content.ReadAngleRotation();
            OnGround = content.ReadBoolean();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content
                .WriteVarInt(EntityId)
                .Write(Position)
                .WriteAngleRotation(Rotation)
                .Write(OnGround);
        }
    }
}