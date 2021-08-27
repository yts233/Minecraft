using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Packets.Client
{
    public class ClientSettingsPacket : Packet
    {
        public override int PacketId => 0x05;

        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Play;

        public string Locale { get; set; }

        /// <summary>
        /// Client-side render distance, in chunks.
        /// </summary>
        public sbyte ViewDistance { get; set; }

        public ChatMode ChatMode { get; set; }

        /// <summary>
        /// “Colors” multiplayer setting
        /// </summary>
        public bool ChatColors { get; set; }

        public SkinPart DisplayedSkinParts { get; set; }

        public Hand MainHand { get; set; }

        /// <summary>
        /// Disables filtering of text on signs and written book titles.
        /// </summary>
        /// <remarks>Currently always true</remarks>
        public bool DisableTextFiltering { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            Locale = content.ReadString();
            ViewDistance = content.ReadByte();
            ChatMode = (ChatMode)content.ReadVarInt();
            ChatColors = content.ReadBoolean();
            DisplayedSkinParts = (SkinPart)content.ReadUnsignedByte();
            MainHand = (Hand)content.ReadVarInt();
            DisableTextFiltering = content.ReadBoolean();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.Write(Locale)
                .Write(ViewDistance)
                .WriteVarInt((int)ChatMode)
                .Write(ChatColors)
                .Write((byte)DisplayedSkinParts)
                .WriteVarInt((int)MainHand)
                .Write(DisableTextFiltering);
        }
    }
}
