using Minecraft.Protocol.Data;
using System;
using System.Diagnostics;
using System.IO;

namespace Minecraft.Protocol.Packets
{
    public static class PacketHelper
    {
        /// <summary>
        /// 获取流的<see cref="DataPacket" />形式
        /// </summary>
        /// <param name="packet">总为null</param>
        /// <param name="stream">流</param>
        /// <returns></returns>
        internal static ByteArray GetContent(this Packet packet, Stream stream)
        {
            return DataTypeHelper.GetContent(null, stream);
        }

        public static Packet Parse(this DataPacket dataPacket)
        {
            var packet = Packet.CreatePacket(dataPacket.PacketId, dataPacket.BoundTo, dataPacket.State);
            try
            {
                return packet.ReadFromStream(dataPacket.Content);
            }
            catch (Exception ex)
            {
                throw new ProtocolException($"Error while processing {packet.GetType().FullName}", ex);
            }
        }

        public static T Parse<T>(this DataPacket dataPacket) where T : Packet, new()
        {
            return (T)new T().ReadFromStream(dataPacket.Content);
        }
    }
}