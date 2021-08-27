using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Client
{
    /// <summary>
    /// Keep-alive packet
    /// </summary>
    /// <remarks>The server will frequently send out a keep-alive, each containing a random ID. The client must respond with the same packet. </remarks>
    public class KeepAliveResponsePacket : Packet
    {
        public override int PacketId => 0x0F;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        public long KeepAliveId { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            KeepAliveId = content.ReadLong();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.Write(KeepAliveId);
        }
    }
}
