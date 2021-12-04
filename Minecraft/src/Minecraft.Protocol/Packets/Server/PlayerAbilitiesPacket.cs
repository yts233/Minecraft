using Minecraft.Protocol.Codecs;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Server
{
    public class PlayerAbilitiesPacket : Packet
    {
        public override int PacketId => 0x32;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        public PlayerAbilitiy Flags { get; set; }

        /// <summary>
        /// The flying speed.
        /// </summary>
        /// <remarks>0.05 by default.</remarks>
        public float FlyingSpeed { get; set; }

        /// <summary>
        /// The fov modifier.
        /// </summary>
        /// <remarks>Modifies the field of view, like a speed potion.</remarks>
        /// <remarks>A Notchian server will use the same value as the movement speed sent in the Entity Properties packet, which defaults to 0.1 for players.</remarks>
        public float FieldOfViewModifier { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Flags = content.ReadEnum<PlayerAbilitiy>();
            FlyingSpeed = content.ReadSingle();
            FieldOfViewModifier = content.ReadSingle();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Flags);
            content.Write(FlyingSpeed);
            content.Write(FieldOfViewModifier);
        }
    }
}
