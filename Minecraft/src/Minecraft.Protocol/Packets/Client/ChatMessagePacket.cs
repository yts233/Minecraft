using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Client
{
    public class ChatMessagePacket : Packet
    {
        public override int PacketId => 0x03;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        public string Message { get; set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            Message = content.ReadString();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write(Message);
        }
    }
}
