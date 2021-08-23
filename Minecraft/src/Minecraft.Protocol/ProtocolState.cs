namespace Minecraft.Protocol
{
    /// <summary>
    /// 协议状态
    /// </summary>
    public enum ProtocolState
    {
        Closed = -2,

        /// <summary>
        /// 任何状态
        /// </summary>
        Any = -1,

        /// <summary>
        /// 握手
        /// </summary>
        Handshaking = 0,

        /// <summary>
        /// 状态
        /// </summary>
        Status = 1,

        /// <summary>
        /// 登入
        /// </summary>
        Login = 2,

        /// <summary>
        /// 游玩
        /// </summary>
        Play = 3
    }
}