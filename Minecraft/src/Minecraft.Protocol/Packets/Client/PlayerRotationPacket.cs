using Minecraft.Data.Numerics;
using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    public class PlayerRotationPacket : Packet
    {
        public override int PacketId => 0x13;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

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
            Rotation = new Rotation { Yaw = content.ReadFloat(), Pitch = content.ReadFloat() };
            OnGround = content.ReadBoolean();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write(Rotation.Yaw)
                .Write(Rotation.Pitch)
                .Write(OnGround);
        }
    }
}
