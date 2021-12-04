using Minecraft.Protocol.Codecs;
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

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Message = content.ReadString();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Message);
        }
    }
}
