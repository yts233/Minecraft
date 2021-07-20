using System.IO;
using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets
{
    public static class PacketHelper
    {
        /// <summary>
        ///     获取流的<see cref="DataPacket" />形式
        /// </summary>
        /// <param name="packet">总为null</param>
        /// <param name="stream">流</param>
        /// <returns></returns>
        internal static ByteArray GetContent(this Packet packet, Stream stream)
        {
            return DataTypeHelper.GetContent(null, stream);
        }

        internal static Packet Parse(this DataPacket dataPacket)
        {
            return Packet
                .CreatePacket(dataPacket.PacketId, dataPacket.BoundTo, dataPacket.State)
                .ReadFromStream(dataPacket.Content);
        }

        internal static T Parse<T>(this DataPacket dataPacket) where T : Packet, new()
        {
            return (T) new T().ReadFromStream(dataPacket.Content);
        }
    }
}