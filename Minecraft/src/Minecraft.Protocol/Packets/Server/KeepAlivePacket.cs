using Minecraft.Protocol.Codecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Server
{
    /// <summary>
    /// Keep-alive packet
    /// </summary>
    /// <remarks>
    /// The server will frequently send out a keep-alive, each containing a random ID. The client must respond with the same payload (see serverbound Keep Alive). If the client does not respond to them for over 30 seconds, the server kicks the client. Vice versa, if the server does not send any keep-alives for 20 seconds, the client will disconnect and yields a "Timed out" exception.
    /// The Notchian server uses a system-dependent time in milliseconds to generate the keep alive ID value.
    /// </remarks>
    public class KeepAlivePacket : Packet
    {
        public override int PacketId => 0x21;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        public long KeepAliveId { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            KeepAliveId = content.ReadInt64();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(KeepAliveId);
        }
    }
}
