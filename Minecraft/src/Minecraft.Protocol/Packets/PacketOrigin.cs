namespace Minecraft.Protocol.Packets
{
    /// <summary>
    ///     数据包绑定
    /// </summary>
    /// <remarks>数据包发送到的位置</remarks>
    /// <example>客户端发送一个数据包至服务器，则<see cref="PacketBoundTo"/>为<see cref="Server"/></example>
    public enum PacketBoundTo
    {
        /// <summary>
        ///     客户端
        /// </summary>
        Client = 0,

        /// <summary>
        ///     服务器
        /// </summary>
        Server = 1
    }
}