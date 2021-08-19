using System;

namespace Minecraft.Resources
{
    /// <summary>
    /// 由资源引发的异常
    /// </summary>
    public class ResourceException : Exception
    {
        internal ResourceException(string message) : base(message)
        {
        }

        internal ResourceException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}