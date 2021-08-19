using System;

namespace Minecraft.Protocol.Packets
{
    /// <summary>
    /// 解析数据包而引发的异常
    /// </summary>
    public class PacketParseException : PacketException
    {
        public PacketParseException()
        {
        }

        public PacketParseException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}