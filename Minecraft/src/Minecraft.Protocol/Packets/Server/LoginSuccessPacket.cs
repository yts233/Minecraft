using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Server
{
    public class LoginSuccessPacket : Packet
    {
        public override int PacketId => 0x02;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Login;

        public Uuid Uuid { get; set; }

        public string Username { get; set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            Uuid = content.ReadUuid();
            Username = content.ReadString();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write(Uuid);
            content.Write(Username);
        }

        protected override void VerifyValues()
        {
            if (Username.Length > 16)
                throw new ArgumentOutOfRangeException("username shouldn't be longer than 16 chars.", nameof(Username));
        }
    }
}
