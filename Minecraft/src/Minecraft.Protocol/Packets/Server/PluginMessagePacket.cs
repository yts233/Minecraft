using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Server
{
    // TODO: 编写此类
    public class PluginMessagePacket : Packet
    {
        public override int PacketId => 0x18;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        protected override void ReadFromStream_(ByteArray content)
        {

        }

        protected override void WriteToStream_(ByteArray content)
        {

        }
    }
}
