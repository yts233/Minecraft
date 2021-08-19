namespace Minecraft.Resources
{
    /// <summary>
    /// 翻译
    /// </summary>
    public class Translation : INamedObject
    {
        /// <summary>
        /// 创建翻译
        /// </summary>
        /// <param name="value">值</param>
        public Translation(NamedIdentifier namedIdentifier, string value)
        {
            NamedIdentifier = namedIdentifier;
            Value = value;
        }

        public NamedIdentifier NamedIdentifier { get; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; }
    }
}