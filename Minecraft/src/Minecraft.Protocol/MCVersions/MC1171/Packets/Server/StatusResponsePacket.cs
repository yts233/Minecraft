using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    /// <summary>
    /// 状态响应包
    /// </summary>
    public class StatusResponsePacket : IPacket
    {
        public int PacketId => 0x00;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;
        public ProtocolState State => ProtocolState.Status;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; private set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Content = content.ReadString();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Content);
        }
    }
}