using OpenTK.Mathematics;
using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class VehicleMovePacket : IPacket
    {
        public int PacketId => 0x15;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// Absolute position
        /// </summary>
        public Vector3d Position { get; set; }

        /// <summary>
        /// Absolute rotation, in degrees.
        /// </summary>
        public Vector2 Rotation { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Position = content.ReadVector3d();
            Rotation = content.ReadRotation();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Position);
            content.Write(Rotation);
        }
    }
}
