using System;
using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets
{
    /// <summary>
    ///     数据包的传输介质
    /// </summary>
    internal class DataPacket : Packet, IDisposable
    {
        private int _packetId;

        internal DataPacket(PacketOrigin origin, ProtocolState state = ProtocolState.Any)
        {
            Origin = origin;
            State = state;
        }

        internal DataPacket(int packetId, PacketOrigin origin, ByteArray content,
            ProtocolState state = ProtocolState.Any)
        {
            _packetId = packetId;
            Origin = origin;
            State = state;
            Content = content;
        }

        /// <summary>
        ///     数据包的内容
        /// </summary>
        public ByteArray Content { get; private set; }

        public override int PacketId => _packetId;
        public override PacketOrigin Origin { get; }
        public override ProtocolState State { get; }

        /// <summary>
        ///     释放内容
        /// </summary>
        public void Dispose()
        {
            Content?.Dispose();
        }

        protected override void _ReadFromStream(ByteArray content)
        {
            _packetId = content.Read<VarInt>();
            Content = content.Read<ByteArray>();
        }

        protected override void _WriteToStream(ByteArray content)
        {
            using var buffer = new ByteArray(0);
            buffer.Write((VarInt) _packetId).Write(Content);
            buffer.Position = 0;
            content.Write((VarInt) buffer.Length).Write(buffer);
        }
    }
}