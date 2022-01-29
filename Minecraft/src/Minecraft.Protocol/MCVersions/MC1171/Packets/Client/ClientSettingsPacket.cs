using Minecraft.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    public class ClientSettingsPacket : IPacket
    {
        public int PacketId => 0x05;

        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Play;

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

        public void ReadFromStream(IPacketCodec content)
        {
            Locale = content.ReadString();
            ViewDistance = content.ReadSByte();
            ChatMode = content.ReadVarIntEnum<ChatMode>();
            ChatColors = content.ReadBoolean();
            DisplayedSkinParts = content.ReadEnum<SkinPart>();
            MainHand = content.ReadVarIntEnum<HandSide>();
            DisableTextFiltering = content.ReadBoolean();
        }

        public void WriteToStream(IPacketCodec content)
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
