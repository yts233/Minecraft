namespace Minecraft
{
    /// <summary>
    ///     日志等级
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///     错误，但无法修正
        /// </summary>
        Fatal,

        /// <summary>
        ///     错误，但已处理
        /// </summary>
        Error,

        /// <summary>
        ///     警告
        /// </summary>
        Warn,

        /// <summary>
        ///     信息
        /// </summary>
        Info,

        /// <summary>
        ///     调试
        /// </summary>
        Debug
    }
}