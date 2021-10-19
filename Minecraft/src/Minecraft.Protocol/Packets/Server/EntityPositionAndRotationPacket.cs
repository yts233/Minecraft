using Minecraft.Numerics;
using Minecraft.Protocol.Data;
using System;

namespace Minecraft.Protocol.Packets.Server
{
    public class EntityPositionAndRotationPacket : Packet
    {
        public override int PacketId => 0x2A;
        public override PacketBoundTo BoundTo => PacketBoundTo.Client;
        public override ProtocolState State => ProtocolState.Play;

        public int EntityId { get; set; }

        public Vector3d Delta { get; set; }

        public Rotation Rotation { get; set; }

        /// <summary>
        /// 实体是否在地面上
        /// </summary>
        public bool OnGround { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            EntityId = content.ReadVarInt();
            var delta = new Vector3d { X = content.ReadShort(), Y = content.ReadShort(), Z = content.ReadShort() };
            delta.Scale(0.000244140625/* 1/4096 */);
            Delta = delta;
            Rotation = content.ReadAngleRotation();
            OnGround = content.ReadBoolean();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content
                .WriteVarInt(EntityId)
                .Write((short)(Delta.X * 4096))
                .Write((short)(Delta.Y * 4096))
                .Write((short)(Delta.Z * 4096))
                .WriteAngleRotation(Rotation)
                .Write(OnGround);
        }

        protected override void VerifyValues()
        {
            if (Math.Abs(Delta.X) > 8 || Math.Abs(Delta.Y) > 8 || Math.Abs(Delta.Z) > 8)
                throw new ProtocolException($"The abs(delta) should be less than 8, use {nameof(EntityTeleportPacket)} instead.");
        }
    }
}