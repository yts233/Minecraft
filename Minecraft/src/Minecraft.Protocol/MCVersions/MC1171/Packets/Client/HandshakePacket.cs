using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    /// <summary>
    /// 握手包
    /// </summary>
    public class HandshakePacket : IPacket
    {
        public int PacketId => 0x00;
        public PacketBoundTo BoundTo => PacketBoundTo.Server;

        public ProtocolState State => ProtocolState.Handshaking;

        /// <summary>
        /// 协议版本
        /// </summary>
        public int ProtocolVersion { get; set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort ServerPort { get; set; }

        /// <summary>
        /// 下一协议状态
        /// </summary>
        /// <remarks>只能是<see cref="ProtocolState.Status" />或<see cref="ProtocolState.Login" /></remarks>
        public ProtocolState NextState { get; set; }

        public void ReadFromStream(IPacketCodec content)
        {
            ProtocolVersion = content.ReadVarInt();
            ServerAddress = content.ReadString();
            ServerPort = content.ReadUInt16();
            NextState = content.ReadVarIntEnum<ProtocolState>();
        }

        public void WriteToStream(IPacketCodec content)
        {
            content.WriteVarInt(ProtocolVersion);
            content.Write(ServerAddress);
            content.Write(ServerPort);
            content.WriteVarIntEnum(NextState);
        }
    }
}