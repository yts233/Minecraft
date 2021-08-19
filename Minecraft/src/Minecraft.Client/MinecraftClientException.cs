using System;
using System.Runtime.Serialization;

namespace Minecraft.Client
{
    [Serializable]
    internal class MinecraftClientException : Exception
    {
        public MinecraftClientException()
        {
        }

        public MinecraftClientException(string message) : base(message)
        {
        }

        public MinecraftClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MinecraftClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}