using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets.Client
{
    /// <summary>
    /// 握手包
    /// </summary>
    public class HandshakePacket : Packet
    {
        public override int PacketId => 0x00;
        public override PacketBoundTo BoundTo => PacketBoundTo.Server;

        public override ProtocolState State => ProtocolState.Handshaking;

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

        protected override void _ReadFromStream(ByteArray content)
        {
            ProtocolVersion = content.ReadVarInt();
            ServerAddress = content.ReadString();
            ServerPort = content.ReadUnsignedShort();
            NextState = content.ReadVarIntEnum<ProtocolState>();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            content
                .WriteVarInt(ProtocolVersion)
                .Write(ServerAddress)
                .Write(ServerPort)
                .WriteVarEnum(NextState);
        }
    }
}