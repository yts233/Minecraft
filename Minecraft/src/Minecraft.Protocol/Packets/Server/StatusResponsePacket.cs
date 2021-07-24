using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Server
{
    /// <summary>
    ///     状态响应包
    /// </summary>
    public class StatusResponsePacket : Packet
    {
        public override int PacketId => 0x00;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;
        public override ProtocolState State => ProtocolState.Status;

        /// <summary>
        ///     内容
        /// </summary>
        public string Content { get; private set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            Content = content.Read<String>();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write(Content);
        }
    }
}