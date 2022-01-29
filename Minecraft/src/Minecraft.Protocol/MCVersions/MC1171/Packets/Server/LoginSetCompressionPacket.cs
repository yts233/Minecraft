using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class LoginSetCompressionPacket : IPacket
    {
        public int PacketId => 0x03;
        public PacketBoundTo BoundTo => PacketBoundTo.Client;
        public ProtocolState State => ProtocolState.Login;

        /// <summary>
        /// Maximum size of a packet before it is compressed. 
        /// </summary>
        public int Threshold { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Threshold = content.ReadVarInt();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.WriteVarInt(Threshold);
        }
    }
}