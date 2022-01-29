namespace Minecraft.Protocol.Packets
{
    public interface IPacketProvider
    {
        bool TryCreatePacket(int packetId, PacketBoundTo boundTo, ProtocolState state, out IPacket packet);
    }
}
