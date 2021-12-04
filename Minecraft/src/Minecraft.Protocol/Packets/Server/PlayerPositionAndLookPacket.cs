using Minecraft.Numerics;
using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Server
{
    public class PlayerPositionAndLookPacket : Packet
    {
        public override int PacketId => 0x38;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The position
        /// </summary>
        public Vector3d Position { get; set; }

        /// <summary>
        /// The rotation in degrees
        /// </summary>
        public Rotation Rotation { get; set; }

        /// <summary>
        /// Absolute or relative position, depending on <see cref="CoordKind"/>. 
        /// </summary>
        public CoordKind XKind { get; set; }

        /// <summary>
        /// Absolute or relative position, depending on <see cref="CoordKind"/>. 
        /// </summary>
        public CoordKind YKind { get; set; }

        /// <summary>
        /// Absolute or relative position, depending on <see cref="CoordKind"/>. 
        /// </summary>
        public CoordKind ZKind { get; set; }

        /// <summary>
        /// Absolute or relative rotation on the Y axis, depending on <see cref="CoordKind"/>. 
        /// </summary>
        public CoordKind YRotKind { get; set; }

        /// <summary>
        /// Absolute or relative rotation on the X axis, depending on <see cref="CoordKind"/>. 
        /// </summary>
        public CoordKind XRotKind { get; set; }

        /// <summary>
        /// The teleport id.
        /// </summary>
        /// <remarks>Client should confirm this packet with <see cref="TeleportConfirmPacket"/> containing the same Teleport ID.</remarks>
        public int TeleportId { get; set; }

        /// <summary>
        /// Dismount vehicle.
        /// </summary>
        /// <remarks>True if the player should dismount their vehicle.</remarks>
        public bool DismountVehicle { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Position = content.ReadVector3d();
            Rotation = content.ReadRotation();
            var flags = content.ReadSByte();
            XKind = (CoordKind)(flags & 0x01);
            YKind = (CoordKind)(flags >> 1 & 0x01);
            ZKind = (CoordKind)(flags >> 2 & 0x01);
            YRotKind = (CoordKind)(flags >> 3 & 0x01);
            XRotKind = (CoordKind)(flags >> 4 & 0x01);
            TeleportId = content.ReadVarInt();
            DismountVehicle = content.ReadBoolean();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            byte flags = 0;
            void PushFlag(CoordKind kind)
            {
                flags |= (byte)(flags << 1 & (byte)kind);
            }
            PushFlag(XKind);
            PushFlag(YKind);
            PushFlag(ZKind);
            PushFlag(YRotKind);
            PushFlag(XRotKind);
            content.Write(Position);
            content.Write(Rotation);
            content.Write(flags);
            content.WriteVarInt(TeleportId);
            content.Write(DismountVehicle);
        }
    }
}
