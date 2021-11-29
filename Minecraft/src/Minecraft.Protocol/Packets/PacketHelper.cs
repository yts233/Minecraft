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

        [Obsolete]
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

        /// <summary>
        /// 尝试创建数据包，并解析
        /// </summary>
        /// <remarks>假如失败，则packet将是dataPacket</remarks>
        /// <param name="dataPacket"></param>
        /// <param name="packet"></param>
        /// <returns>是否成功创建数据包</returns>
        /// <exception cref="ProtocolException"></exception>
        public static bool TryParse(this DataPacket dataPacket, out Packet packet)
        {
            if (!Packet.TryCreatePacket(dataPacket.PacketId, dataPacket.BoundTo, dataPacket.State, out packet))
            {
                packet = dataPacket;
                return false;
            }
            try
            {
                packet.ReadFromStream(dataPacket.Content);
            }
            catch (Exception ex)
            {
                throw new ProtocolException($"Error while processing {packet.GetType().FullName}", ex);
            }
            return true;
        }

        public static T Parse<T>(this DataPacket dataPacket) where T : Packet, new()
        {
            return (T)new T().ReadFromStream(dataPacket.Content);
        }
    }
}