using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Server
{
    public class LoginDisconnectPacket : Packet
    {
        public override int PacketId => 0x00;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Login;

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
