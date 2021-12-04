using Minecraft.Numerics;
using Minecraft.Protocol.Codecs;

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

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Position = content.ReadVector3d();
            Rotation = content.ReadRotation();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Position);
            content.Write(Rotation);
        }
    }
}
