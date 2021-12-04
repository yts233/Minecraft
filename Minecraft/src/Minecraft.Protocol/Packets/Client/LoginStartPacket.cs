using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Client
{
    public class LoginStartPacket : Packet
    {
        public override int PacketId => 0x00;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Login;

        /// <summary>
        /// Player's Username
        /// </summary>
        public string Name { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Name = content.ReadString();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Name);
        }
    }
}
