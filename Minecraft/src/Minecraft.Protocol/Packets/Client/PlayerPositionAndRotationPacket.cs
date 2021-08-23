using Minecraft.Data.Numerics;
using Minecraft.Protocol.Data;

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

        protected override void _ReadFromStream(ByteArray content)
        {
            Position = new Vector3d { X = content.ReadDouble(), Y = content.ReadDouble(), Z = content.ReadDouble() };
            Rotation = new Rotation { Yaw = content.ReadFloat(), Pitch = content.ReadFloat() };
            OnGround = content.ReadBoolean();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write(Position.X)
                .Write(Position.Y)
                .Write(Position.Z)
                .Write(Rotation.Yaw)
                .Write(Rotation.Pitch)
                .Write(OnGround);
        }
    }
}
