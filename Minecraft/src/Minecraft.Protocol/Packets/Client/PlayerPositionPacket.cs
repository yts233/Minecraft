using Minecraft.Numerics;
using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Client
{
    public class PlayerPositionPacket : Packet
    {
        public override int PacketId => 0x11;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The absolute position
        /// </summary>
        /// <remarks>The Y is feet Y, normally Head Y - 1.62. </remarks>
        public Vector3d Position { get; set; }

        /// <summary>
        /// True if the client is on the ground, false otherwise.
        /// </summary>
        public bool OnGround { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Position = content.ReadVector3d();
            OnGround = content.ReadBoolean();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Position);
            content.Write(OnGround);
        }
    }
}
