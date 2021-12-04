using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Server
{
    /// <summary>
    /// 状态响应包
    /// </summary>
    public class StatusResponsePacket : Packet
    {
        public override int PacketId => 0x00;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;
        public override ProtocolState State => ProtocolState.Status;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; private set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Content = content.ReadString();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Content);
        }
    }
}