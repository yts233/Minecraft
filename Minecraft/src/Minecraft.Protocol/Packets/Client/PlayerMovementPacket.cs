﻿using Minecraft.Protocol.Codecs;

namespace Minecraft.Protocol.Packets.Client
{
    public class PlayerMovementPacket : Packet
    {
        public override int PacketId => 0x14;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        public bool OnGround { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            OnGround = content.ReadBoolean();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(OnGround);
        }
    }
}
