using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    public class LoginSetCompressionPacket : Packet
    {
        public override int PacketId => 0x03;
        public override PacketBoundTo BoundTo => PacketBoundTo.Client;
        public override ProtocolState State => ProtocolState.Login;

        /// <summary>
        /// Maximum size of a packet before it is compressed. 
        /// </summary>
        public int Threshold { get; set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            Threshold = content.ReadVarInt();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.WriteVar(Threshold);
        }
    }
}