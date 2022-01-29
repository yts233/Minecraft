using Minecraft.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class LoginDisconnectPacket : IPacket
    {
        public int PacketId => 0x00;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Login;

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
