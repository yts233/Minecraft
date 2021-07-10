namespace Minecraft.Resources
{
    /// <summary>
    ///     有名字的资源
    /// </summary>
    public interface INamedResource
    {
        /// <summary>
        ///     命名空间
        /// </summary>
        string Namespace { get; }

        /// <summary>
        ///     名称
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     全名
        /// </summary>
        string Fullname => $"{Namespace}:{Name}";
    }
}