using Minecraft.Protocol.Codecs;
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

        public HandSide MainHand { get; set; }

        /// <summary>
        /// Disables filtering of text on signs and written book titles.
        /// </summary>
        /// <remarks>Currently always true</remarks>
        public bool DisableTextFiltering { get; set; }

        protected override void ReadFromStream_(IPacketCodec content)
        {
            Locale = content.ReadString();
            ViewDistance = content.ReadSByte();
            ChatMode = content.ReadVarIntEnum<ChatMode>();
            ChatColors = content.ReadBoolean();
            DisplayedSkinParts = content.ReadEnum<SkinPart>();
            MainHand = content.ReadVarIntEnum<HandSide>();
            DisableTextFiltering = content.ReadBoolean();
        }

        protected override void WriteToStream_(IPacketCodec content)
        {
            content.Write(Locale);
            content.Write(ViewDistance);
            content.WriteVarIntEnum(ChatMode);
            content.Write(ChatColors);
            content.Write(DisplayedSkinParts);
            content.WriteVarIntEnum(MainHand);
            content.Write(DisableTextFiltering);
        }
    }
}
