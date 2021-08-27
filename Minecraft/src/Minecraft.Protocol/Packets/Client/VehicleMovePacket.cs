using Minecraft.Numerics;
using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Client
{
    public class VehicleMovePacket : Packet
    {
        public override int PacketId => 0x15;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// Absolute position
        /// </summary>
        public Vector3d Position { get; set; }

        /// <summary>
        /// Absolute rotation, in degrees.
        /// </summary>
        public Rotation Rotation { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            Position = content.ReadVector3d();
            Rotation = content.ReadRotation();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.Write(Position)
                .Write(Rotation);
        }
    }
}
