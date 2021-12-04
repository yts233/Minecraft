using Minecraft.Numerics;
using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Server
{
    public class EntityRotationPacket : Packet
    {
        public override int PacketId => 0x2B;
        public override PacketBoundTo BoundTo => PacketBoundTo.Client;
        public override ProtocolState State => ProtocolState.Play;

        public int EntityId { get; set; }

        public Rotation Rotation { get; set; }

        /// <summary>
        /// 实体是否在地面上
        /// </summary>
        public bool OnGround { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            EntityId = content.ReadVarInt();
            Rotation = content.ReadAngleRotation();
            OnGround = content.ReadBoolean();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.WriteVarInt(EntityId);
            content.WriteAngleRotation(Rotation);
            content.Write(OnGround);
        }
    }
}