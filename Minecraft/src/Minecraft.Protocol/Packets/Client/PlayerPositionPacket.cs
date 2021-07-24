using Minecraft.Data.Numerics;
using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
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
        /// On ground
        /// </summary>
        /// <remarks>True if the client is on the ground, false otherwise.</remarks>
        public bool OnGround { get; set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            Position = new Vector3d { X = content.ReadDouble(), Y = content.ReadDouble(), Z = content.ReadDouble() };
            OnGround = content.ReadBoolean();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write(Position.X)
                .Write(Position.Y)
                .Write(Position.Z)
                .Write(OnGround);
        }
    }
}
