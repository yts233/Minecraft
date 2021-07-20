using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    /// <summary>
    ///     实体移动包
    /// </summary>
    public class EntityMovementPacket : Packet
    {
        public override int PacketId => 0x28;
        public override PacketBoundTo BoundTo => PacketBoundTo.Client;
        public override ProtocolState State => ProtocolState.Play;

        public int EntityId { get; private set; }

        /// <summary>
        ///     Change in X position as (currentX * 32 - prevX * 32) * 128
        /// </summary>
        public short DeltaX { get; private set; }

        /// <summary>
        ///     Change in Y position as (currentY * 32 - prevY * 32) * 128
        /// </summary>
        public short DeltaY { get; private set; }

        /// <summary>
        ///     Change in Z position as (currentZ * 32 - prevZ * 32) * 128
        /// </summary>
        public short DeltaZ { get; private set; }

        /// <summary>
        ///     实体是否在地面上
        /// </summary>
        public bool OnGround { get; private set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            EntityId = content.ReadVarInt();
            DeltaX = content.ReadShort();
            DeltaY = content.ReadShort();
            DeltaZ = content.ReadShort();
            OnGround = content.ReadBoolean();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content
                .WriteVar(EntityId)
                .Write(DeltaX)
                .Write(DeltaY)
                .Write(DeltaZ)
                .Write(OnGround);
        }
    }
}