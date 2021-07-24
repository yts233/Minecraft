using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Server
{
    public class ChatMessagePacket : Packet
    {
        public override int PacketId => 0xFF0F;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// Chat message
        /// </summary>
        /// <remarks>Limited to 262144 bytes.</remarks>
        public string JsonData { get; set; }

        public ChatMessagePosition Position { get; set; }

        public Uuid Sender { get; set; }

        protected override void _ReadFromStream(ByteArray content)
        {
            JsonData = content.ReadString();
            Position = (ChatMessagePosition)content.ReadByte();
            Sender = content.ReadUuid();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content.Write(JsonData)
                .Write((sbyte)Position)
                .Write(Sender);
        }
    }
}
