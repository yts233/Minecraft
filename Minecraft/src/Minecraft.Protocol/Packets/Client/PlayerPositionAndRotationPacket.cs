using Minecraft.Numerics;
using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Client
{
    /// <summary>
    /// A combination of <see cref="PlayerRotationPacket"/> and <see cref="PlayerPositionPacket"/>.
    /// </summary>
    public class PlayerPositionAndRotationPacket : Packet
    {
        public override int PacketId => 0x12;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The absolute position
        /// </summary>
        /// <remarks>The Y is feet Y, normally Head Y - 1.62. </remarks>
        public Vector3d Position { get; set; }

        /// <summary>
        /// The absolute rotation in degrees
        /// </summary>
        public Rotation Rotation { get; set; }

        /// <summary>
        /// On ground
        /// </summary>
        /// <remarks>True if the client is on the ground, false otherwise.</remarks>
        public bool OnGround { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Position = content.ReadVector3d();
            Rotation = content.ReadRotation();
            OnGround = content.ReadBoolean();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Position);
            content.Write(Rotation);
            content.Write(OnGround);
        }
    }
}
