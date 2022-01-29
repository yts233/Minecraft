using Minecraft.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    /// <summary>
    /// Keep-alive packet
    /// </summary>
    /// <remarks>The server will frequently send out a keep-alive, each containing a random ID. The client must respond with the same packet. </remarks>
    public class KeepAliveResponsePacket : IPacket
    {
        public int PacketId => 0x0F;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

        public long KeepAliveId { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            KeepAliveId = content.ReadInt64();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(KeepAliveId);
        }
    }
}
