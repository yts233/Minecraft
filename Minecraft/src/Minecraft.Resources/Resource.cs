using System;
using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Resources
{
    /// <summary>
    /// 提供<see cref="Asset" />的资源
    /// </summary>
    public abstract class Resource : IDisposable, IAssetProvider
    {
        private static readonly Logger<Resource> _logger = Logger.GetLogger<Resource>();

        /// <summary>
        /// 创建资源
        /// </summary>
        /// <param name="id">标识符</param>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="argument">装载资源所需要的参数</param>
        protected Resource(Uuid id, string name = "a general resource", string description = null,
            object argument = null)
        {
            Id = id;
            Name = name;
            Description = description;
            // ReSharper disable once VirtualMemberCallInConstructor
            LoadAssets(argument);
            _logger.Info($"Loaded resource {Name ?? "unnamed"}: {Description}");
        }

        /// <summary>
        /// 资源标识符
        /// </summary>
        /// <value>The identifier.</value>
        public Uuid Id { get; }

        /// <summary>
        /// 资源名称
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// 资源描述
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; protected set; }

        /// <summary>
        /// Asset数量
        /// </summary>
        /// <value>The count.</value>
        public abstract int Count { get; }

        public Asset this[AssetType type, NamedIdentifier name]
        {
            get
            {
                var asset = GetAssets().FirstOrDefault(p => p.Type == type && p.NamedIdentifier.Equals(name));
                if (asset == null)
                    throw new ResourceException($"{type} asset {name} not found!");
                return asset;
            }
        }

        public virtual void Dispose()
        {
        }

        /// <summary>
        /// 用于加载资源
        /// </summary>
        protected abstract void LoadAssets(object argument4);

        /// <summary>
        /// 获取<see cref="Asset" />
        /// </summary>
        /// <returns>The enumerator.</returns>
        public abstract IEnumerable<Asset> GetAssets();

        /// <summary>
        /// 获取<see cref="Language" />
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Language> GetLanguages();

        public override string ToString()
        {
            return $"{Name} - {Description} ({Count} assets)";
        }

        public bool TryGetAsset(AssetType type, NamedIdentifier name, out Asset asset)
        {
            asset = GetAssets().FirstOrDefault(p => p.Type == type && p.NamedIdentifier == name);
            return asset != null;
        }
    }
}