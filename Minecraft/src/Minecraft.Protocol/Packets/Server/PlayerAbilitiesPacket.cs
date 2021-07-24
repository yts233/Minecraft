using Minecraft.Protocol.Data;
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

        protected override void _ReadFromStream(ByteArray content)
        {
            Flags = (PlayerAbilitiy)content.ReadByte();
            FlyingSpeed = content.ReadFloat();
            FieldOfViewModifier = content.ReadFloat();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write((sbyte)Flags)
                .Write(FlyingSpeed)
                .Write(FieldOfViewModifier);
        }
    }
}
