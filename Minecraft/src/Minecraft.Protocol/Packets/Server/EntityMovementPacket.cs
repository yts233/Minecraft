using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    /// <summary>
    ///     实体移动包
    /// </summary>
    public class EntityMovementPacket : Packet
    {
        public override int PacketId => 0x28;
        public override PacketOrigin Origin => PacketOrigin.Server;
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
            EntityId = content.Read<VarInt>();
            DeltaX = content.Read<Short>();
            DeltaY = content.Read<Short>();
            DeltaZ = content.Read<Short>();
            OnGround = content.Read<Boolean>();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content
                .Write((VarInt) EntityId)
                .Write((Short) DeltaX)
                .Write((Short) DeltaY)
                .Write((Short) DeltaZ)
                .Write((Boolean) OnGround);
        }
    }
}