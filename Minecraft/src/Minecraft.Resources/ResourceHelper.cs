using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Resources
{
    /// <summary>
    /// 资源助手
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// 从<see cref="Resource" />获取<see cref="Asset" />
        /// </summary>
        /// <param name="resource">资源</param>
        /// <param name="type">类型</param>
        /// <param name="fullname">全名</param>
        /// <returns></returns>
        public static Asset GetAsset(this Resource resource, AssetType type, NamedIdentifier fullname)
        {
            return resource
                .GetAssets()
                .Where(asset => asset.Type == type)
                .FirstOrDefault(asset => asset.NamedIdentifier == fullname);
        }

        public static Language GetLanguage(this Resource resource, string id)
        {
            return resource
                .GetLanguages()
                .FirstOrDefault(language => language.Id == id);
        }

        public static Translation GetTranslation(this IEnumerable<Translation> translations, NamedIdentifier fullname)
        {
            return translations
                .FirstOrDefault(translation => translation.NamedIdentifier == fullname);
        }

        public static string GetValue(this IEnumerable<Translation> translations, string fullname)
        {
            return translations.GetTranslation(fullname)
                .Value;
        }
    }
}