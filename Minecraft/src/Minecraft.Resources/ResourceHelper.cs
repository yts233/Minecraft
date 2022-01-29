using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Resources
{
    /// <summary>
    /// 资源助手
    /// </summary>
    public static class ResourceHelper
    {
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

        public static Asset GetFile(this IAssetProvider assetProvider, NamedIdentifier name)
        {
            if (name.Name.IndexOf('/') == -1)
                assetProvider.GetAssets().FirstOrDefault(p => p.NamedIdentifier.Equals(name));
            var type = name.Name[..name.Name.IndexOf('/')] switch
            {
                "blockstates" => AssetType.Blockstate,
                "font" => AssetType.Font,
                "icons" => AssetType.Icon,
                "lang" => AssetType.Lang,
                "models" => AssetType.Model,
                "shaders" => AssetType.Shader,
                "sounds" => AssetType.Sound,
                "texts" => AssetType.Text,
                "textures" => AssetType.Texture,
                _ => (AssetType)(-1)
            };
            if ((int)type == -1 || !assetProvider.TryGetAsset(type, new NamedIdentifier(name.Namespace, name.Name[(name.Name.IndexOf('/') + 1)..]), out var asset))
            {
                asset = assetProvider.GetAssets().FirstOrDefault(p => p.NamedIdentifier.Equals(name));
            }
            return asset;
        }
    }
}