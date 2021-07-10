using System;

namespace Minecraft.Protocol
{
    /// <summary>
    ///     协议引发的异常
    /// </summary>
    public class ProtocolException : Exception
    {
        internal ProtocolException()
        {
        }

        internal ProtocolException(string message) : base(message)
        {
        }

        internal ProtocolException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}