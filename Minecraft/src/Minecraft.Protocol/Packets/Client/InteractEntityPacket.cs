using Minecraft.Numerics;
using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Client
{
    public class InteractEntityPacket : Packet
    {
        public override int PacketId => 0x0D;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        public int EntityId { get; set; }

        public InteractType Type { get; set; }

        /// <summary>
        /// The interact target
        /// </summary>
        /// <remarks>Only if <see cref="Type"/> is <see cref="InteractType.InteractAt"/></remarks>
        public Vector3f Target { get; set; }

        /// <summary>
        /// The interact hand
        /// </summary>
        /// <remarks>Only if <see cref="Type"/> is <see cref="InteractType.Interact"/> or <see cref="InteractType.InteractAt"/></remarks>
        public Hand Hand { get; set; }

        public bool Sneaking { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            EntityId = content.ReadVarInt();
            Type = content.ReadVarIntEnum<InteractType>();
            if (Type == InteractType.InteractAt)
                Target = content.ReadVector3f();
            if (Type == InteractType.Interact || Type == InteractType.InteractAt)
                Hand = content.ReadVarIntEnum<Hand>();
            Sneaking = content.ReadBoolean();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.WriteVarInt(EntityId);
            content.WriteVarIntEnum(Type);
            if (Type == InteractType.InteractAt)
                content.Write(Target);
            if (Type == InteractType.Interact || Type == InteractType.InteractAt)
                content.WriteVarIntEnum(Hand);
            content.Write(Sneaking);
        }
    }
}
