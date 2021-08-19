using System;

namespace Minecraft.Protocol.Packets
{
    /// <summary>
    /// 由数据包引发的异常
    /// </summary>
    public abstract class PacketException : ProtocolException
    {
        protected PacketException()
        {
        }

        protected PacketException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}