namespace Minecraft
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 错误，但无法修正
        /// </summary>
        Fatal = 0,

        /// <summary>
        /// 错误，但已处理
        /// </summary>
        Error = 1,

        /// <summary>
        /// 警告
        /// </summary>
        Warn = 2,

        /// <summary>
        /// 信息
        /// </summary>
        Info = 3,

        /// <summary>
        /// 调试
        /// </summary>
        Debug = 4
    }
}