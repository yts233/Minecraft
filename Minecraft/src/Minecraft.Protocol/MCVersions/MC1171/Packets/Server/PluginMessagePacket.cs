using Minecraft.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    // TODO: 编写此类
    public class PluginMessagePacket : IPacket
    {
        public int PacketId => 0x18;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Play;

        public void ReadFromStream(IPacketCodec content)
        {

        }

        public void WriteToStream(IPacketCodec content)
        {

        }
    }
}
