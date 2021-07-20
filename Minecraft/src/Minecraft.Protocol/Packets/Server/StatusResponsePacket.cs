using System;
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
        public override ProtocolState State => ProtocolState.Handshaking;

        /// <summary>
        ///     内容
        /// </summary>
        public string Content { get; private set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            Content = content.Read<VarChar>();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.WriteVar(Content);
        }
    }
}