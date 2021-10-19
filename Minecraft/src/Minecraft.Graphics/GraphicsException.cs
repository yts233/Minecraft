using System;

namespace Minecraft.Graphics
{
    /// <summary>
    /// 图形引发的异常
    /// </summary>
    public class GraphicException : Exception
    {
        public GraphicException(string message) : base(message)
        {
        }
    }
}