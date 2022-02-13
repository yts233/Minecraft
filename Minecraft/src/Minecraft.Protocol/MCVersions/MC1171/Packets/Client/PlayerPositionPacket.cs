using OpenTK.Mathematics;
using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class PlayerPositionPacket : IPacket
    {
        public int PacketId => 0x11;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The absolute position
        /// </summary>
        /// <remarks>The Y is feet Y, normally Head Y - 1.62. </remarks>
        public Vector3d Position { get; set; }

        /// <summary>
        /// True if the client is on the ground, false otherwise.
        /// </summary>
        public bool OnGround { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Position = content.ReadVector3d();
            OnGround = content.ReadBoolean();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Position);
            content.Write(OnGround);
        }
    }
}
