using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171.Packets.Client
{
    /// <summary>
    /// 状态请求包
    /// </summary>
    public class StatusRequestPacket : IPacket
    {
        public int PacketId => 0x00;
        public PacketBoundTo BoundTo => PacketBoundTo.Server;
        public ProtocolState State => ProtocolState.Status;

        public void ReadFromStream(IPacketCodec content)
        {
            //empty
        }

        public void WriteToStream(IPacketCodec content)
        {
            //empty
        }
    }
}