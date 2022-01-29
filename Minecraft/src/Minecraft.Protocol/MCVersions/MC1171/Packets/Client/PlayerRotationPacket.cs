using Minecraft.Numerics;
using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class PlayerRotationPacket : IPacket
    {
        public int PacketId => 0x13;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The absolute rotation in degrees
        /// </summary>
        public Rotation Rotation { get; set; }

        /// <summary>
        /// On ground
        /// </summary>
        /// <remarks>True if the client is on the ground, false otherwise.</remarks>
        public bool OnGround { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Rotation = content.ReadRotation();
            OnGround = content.ReadBoolean();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Rotation);
            content.Write(OnGround);
        }
    }
}
