using Minecraft.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class ChatMessagePacket : IPacket
    {
        public int PacketId => 0x03;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        public string Message { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Message = content.ReadString();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Message);
        }
    }
}
