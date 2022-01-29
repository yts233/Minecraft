using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class LoginStartPacket : IPacket
    {
        public int PacketId => 0x00;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Login;

        /// <summary>
        /// Player's Username
        /// </summary>
        public string Name { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Name = content.ReadString();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Name);
        }
    }
}
