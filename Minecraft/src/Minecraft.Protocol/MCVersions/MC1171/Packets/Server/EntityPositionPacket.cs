using OpenTK.Mathematics;
using Minecraft.Protocol.Packets;
using System;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class EntityPositionPacket : IPacket
    {
        public int PacketId => 0x29;
        public PacketBoundTo BoundTo => PacketBoundTo.Client;
        public ProtocolState State => ProtocolState.Play;

        public int EntityId { get; set; }

        public Vector3d Delta { get; set; }

        /// <summary>
        /// 实体是否在地面上
        /// </summary>
        public bool OnGround { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            EntityId = content.ReadVarInt();
            var delta = new Vector3d { X = content.ReadInt16(), Y = content.ReadInt16(), Z = content.ReadInt16() };
            delta *= 0.000244140625/* 1/4096 */;
            Delta = delta;
            OnGround = content.ReadBoolean();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.WriteVarInt(EntityId);
            content.Write((short)(Delta.X * 4096));
            content.Write((short)(Delta.Y * 4096));
            content.Write((short)(Delta.Z * 4096));
            content.Write(OnGround);
        }

        public void VerifyValues()
        {
            if (Math.Abs(Delta.X) > 8 || Math.Abs(Delta.Y) > 8 || Math.Abs(Delta.Z) > 8)
                throw new ProtocolException($"The abs(delta) should be less than 8, use {nameof(EntityTeleportPacket)} instead.");
        }
    }
}