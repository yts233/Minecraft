using Minecraft.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class DisconnectPacket : IPacket
    {
        public int PacketId => 0x1A;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Any;

        /// <summary>
        /// The reason
        /// </summary>
        /// <remarks>Displayed to the client when the connection terminates.</remarks>
        public string Reason { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Reason = content.ReadString();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Reason);
        }
    }
}
