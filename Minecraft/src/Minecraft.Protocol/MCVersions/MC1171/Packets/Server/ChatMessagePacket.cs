#define FixEndOfStream
using Minecraft.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Server
{
    public class ChatMessagePacket : IPacket
    {
        public int PacketId => 0x0F;

        public PacketBoundTo BoundTo => PacketBoundTo.Client;

        public ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// Chat message
        /// </summary>
        /// <remarks>Limited to 262144 bytes.</remarks>
        public string JsonData { get; set; }

        public ChatMessagePosition Position { get; set; }

        public Uuid Sender { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {

#if FixEndOfStream
            try
            {
                JsonData = content.ReadString();
                Position = content.ReadEnum<ChatMessagePosition>();
                Sender = content.ReadUuid();
            }
            catch
            {
                // ignore
            }
#else
            JsonData = content.ReadString();
            Position = content.ReadEnum<ChatMessagePosition>();
            Sender = content.ReadUuid();
#endif
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.Write(JsonData);
            content.Write(Position);
            content.Write(Sender);
        }
    }
}
