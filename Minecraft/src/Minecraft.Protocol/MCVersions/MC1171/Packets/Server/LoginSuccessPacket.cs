using Minecraft.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class LoginSuccessPacket : IPacket
    {
        public int PacketId => 0x02;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Login;

        public Uuid Uuid { get; set; }

        public string Username { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            Uuid = content.ReadUuid();
            Username = content.ReadString();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(Uuid);
            content.Write(Username);
        }

        public void VerifyValues()
        {
            if (Username.Length > 16)
                throw new ArgumentOutOfRangeException("username shouldn't be longer than 16 chars.", nameof(Username));
        }
    }
}
