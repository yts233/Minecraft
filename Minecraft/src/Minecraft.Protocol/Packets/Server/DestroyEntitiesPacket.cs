using Minecraft.Protocol.Data;
using System.Linq;

namespace Minecraft.Protocol.Packets.Server
{
    public class DestroyEntitiesPacket : Packet
    {
        public override int PacketId => 0x3A;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// Number of elements in <see cref="EntityIds"/>.
        /// </summary>
        /// <remarks>Unnessary when send this packet.</remarks>
        public int Count { get; set; }

        public int[] EntityIds { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            Count = content.ReadVarInt();
            EntityIds = content.ReadArray<VarInt, int>(Count);
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.Write(Count)
                .WriteArray(EntityIds.Select(ele => (VarInt)ele).ToArray());
        }

        protected override void VerifyValues()
        {
            Count = EntityIds.Length;
        }
    }
}