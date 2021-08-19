using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Resources
{
    /// <summary>
    /// 共享的资源
    /// </summary>
    public static class SharedResource
    {
        private static readonly ICollection<Resource> Resources = new List<Resource>();
        private static IEnumerable<Asset> _assets = new List<Asset>();
        private static IEnumerable<Language> _languages = new List<Language>();

        /// <summary>
        /// 重新遍历<see cref="Asset" />
        /// </summary>
        public static void Reload()
        {
            _assets = Resources
                .SelectMany(resource => resource.GetAssets())
                .ToList();
            _languages = Resources
                .SelectMany(resource => resource.GetLanguages())
                .ToList();
        }

        /// <summary>
        /// 激活资源
        /// </summary>
        /// <param name="resource">资源</param>
        public static void Activate(this Resource resource)
        {
            if (!Resources.Contains(resource))
                Resources.Add(resource);
        }

        /// <summary>
        /// 取消激活资源
        /// </summary>
        /// <param name="resource">资源</param>
        public static void Deactivate(this Resource resource)
        {
            Resources.Remove(resource);
        }

        /// <summary>
        /// 获取<see cref="Asset" />
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="fullname">全名</param>
        /// <returns></returns>
        public static Asset GetAsset(AssetType type, NamedIdentifier fullname)
        {
            return _assets
                .Where(asset => asset.Type == type)
                .First(asset => asset.NamedIdentifier == fullname);
        }

        public static IEnumerable<Translation> GetTranslations(string id)
        {
            return _languages
                .Where(language => language.Id == id)
                .SelectMany(language => language)
                .ToList();
        }
    }
}