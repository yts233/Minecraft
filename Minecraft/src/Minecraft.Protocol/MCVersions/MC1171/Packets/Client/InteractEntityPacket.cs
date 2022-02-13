using OpenTK.Mathematics;
using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class InteractEntityPacket : IPacket
    {
        public int PacketId => 0x0D;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        public int EntityId { get; set; }

        public InteractType Type { get; set; }

        /// <summary>
        /// The interact target
        /// </summary>
        /// <remarks>Only if <see cref="Type"/> is <see cref="InteractType.InteractAt"/></remarks>
        public Vector3 Target { get; set; }

        /// <summary>
        /// The interact hand
        /// </summary>
        /// <remarks>Only if <see cref="Type"/> is <see cref="InteractType.Interact"/> or <see cref="InteractType.InteractAt"/></remarks>
        public Hand Hand { get; set; }

        public bool Sneaking { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            EntityId = content.ReadVarInt();
            Type = content.ReadVarIntEnum<InteractType>();
            if (Type == InteractType.InteractAt)
                Target = content.ReadVector3();
            if (Type == InteractType.Interact || Type == InteractType.InteractAt)
                Hand = content.ReadVarIntEnum<Hand>();
            Sneaking = content.ReadBoolean();
        }

        public void WriteToStream(IPacketCodec content)
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
