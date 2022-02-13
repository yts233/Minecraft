using OpenTK.Mathematics;
using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    /// <summary>
    /// A combination of <see cref="PlayerRotationPacket"/> and <see cref="PlayerPositionPacket"/>.
    /// </summary>
    public class PlayerPositionAndRotationPacket : IPacket
    {
        public int PacketId => 0x12;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The absolute position
        /// </summary>
        /// <remarks>The Y is feet Y, normally Head Y - 1.62. </remarks>
        public Vector3d Position { get; set; }

        /// <summary>
        /// The absolute rotation in degrees
        /// </summary>
        public Vector2 Rotation { get; set; }

        /// <summary>
        /// On ground
        /// </summary>
        /// <remarks>True if the client is on the ground, false otherwise.</remarks>
        public bool OnGround { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Position = content.ReadVector3d();
            Rotation = content.ReadRotation();
            OnGround = content.ReadBoolean();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Position);
            content.Write(Rotation);
            content.Write(OnGround);
        }
    }
}
