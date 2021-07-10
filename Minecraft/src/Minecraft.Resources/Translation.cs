namespace Minecraft.Resources
{
    /// <summary>
    ///     翻译
    /// </summary>
    public class Translation : INamedResource
    {
        /// <summary>
        ///     创建翻译
        /// </summary>
        /// <param name="namespace">命名空间</param>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public Translation(string @namespace, string name, string value)
        {
            Namespace = @namespace;
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     全名
        /// </summary>
        public string FullName => $"{Namespace}:{Name}";

        /// <summary>
        ///     值
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     命名空间
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        ///     名称
        /// </summary>
        public string Name { get; }
    }
}