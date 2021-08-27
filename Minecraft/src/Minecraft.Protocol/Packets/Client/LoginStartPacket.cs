using Minecraft.Protocol.Data;

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

        protected override void ReadFromStream_(ByteArray content)
        {
            Name = content.ReadString();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.Write(Name);
        }
    }
}
