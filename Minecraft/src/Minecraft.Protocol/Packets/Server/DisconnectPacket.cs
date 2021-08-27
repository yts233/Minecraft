using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Server
{
    public class DisconnectPacket : Packet
    {
        public override int PacketId => 0x1A;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The reason
        /// </summary>
        /// <remarks>Displayed to the client when the connection terminates.</remarks>
        public string Reason { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            Reason = content.ReadString();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.Write(Reason);
        }
    }
}
