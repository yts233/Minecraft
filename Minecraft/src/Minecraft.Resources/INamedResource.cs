namespace Minecraft.Resources
{
    /// <summary>
    /// 有名字的资源
    /// </summary>
    public interface INamedObject
    {
        /// <summary>
        /// 命名Id
        /// </summary>
        NamedIdentifier NamedIdentifier { get; }

        /// <summary>
        /// 命名空间
        /// </summary>
        string Namespace => NamedIdentifier.Namespace;

        /// <summary>
        /// 名称
        /// </summary>
        string Name => NamedIdentifier.Name;

        /// <summary>
        /// 全名
        /// </summary>
        string Fullname => NamedIdentifier.FullName;
    }
}