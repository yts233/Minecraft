using Minecraft.Protocol.Packets;
using System.Linq;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class DestroyEntitiesPacket : IPacket
    {
        public int PacketId => 0x3A;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// Number of elements in <see cref="EntityIds"/>.
        /// </summary>
        /// <remarks>Unnessary when send this packet.</remarks>
        public int Count { get; set; }

        public int[] EntityIds { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Count = content.ReadVarInt();
            EntityIds = content.ReadVarInts(Count);
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Count);
            content.WriteVarInts(EntityIds);
        }

        public void VerifyValues()
        {
            Count = EntityIds.Length;
        }
    }
}