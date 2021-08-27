#define FixEndOfStream
using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Server
{
    public class ChatMessagePacket : Packet
    {
        public override int PacketId => 0x0F;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// Chat message
        /// </summary>
        /// <remarks>Limited to 262144 bytes.</remarks>
        public string JsonData { get; set; }

        public ChatMessagePosition Position { get; set; }

        public Uuid Sender { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {

#if FixEndOfStream
            try
            {
                JsonData = content.ReadString();
                Position = (ChatMessagePosition)content.ReadByte();
                Sender = content.ReadUuid();
            }
            catch
            {
                // ignore
            }
#else
            JsonData = content.ReadString();
            Position = (ChatMessagePosition)content.ReadByte();
            Sender = content.ReadUuid();
#endif
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.Write(JsonData)
                .Write((sbyte)Position)
                .Write(Sender);
        }
    }
}
