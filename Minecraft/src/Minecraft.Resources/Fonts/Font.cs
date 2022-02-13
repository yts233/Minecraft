using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Minecraft.Resources.Fonts
{
    public class Font : INamedObject
    {
        private readonly IAssetProvider _assetProvider;
        private readonly NamedIdentifier _id;
        private readonly List<IFontProvider> _providers = new();

        public NamedIdentifier NamedIdentifier => _id;

        public IEnumerable<IFontProvider> Providers => _providers;

        /// <summary>
        /// 加载字体
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="id">如 <code>minecraft:default</code></param>
        public Font(IAssetProvider provider, NamedIdentifier id, bool forceUnicodeFont = false)
        {
            _assetProvider = provider;
            _id = id;
            Load(forceUnicodeFont);
        }

        private void Load(bool forceUnicodeFont)
        {
            using var fontJson = JsonDocument.Parse(_assetProvider[AssetType.Font, _id + ".json"].OpenRead());
            var providers = fontJson.RootElement.GetProperty("providers");
            foreach (var provider in providers.EnumerateArray())
            {
                try
                {
                    var type = provider.GetProperty("type").GetString();
                    if (forceUnicodeFont && type != "legacy_unicode")
                        continue;
                    switch (type)
                    {
                        case "bitmap":
                            provider.TryGetProperty("height", out var height);
                            _providers.Add(
                                new BitmapFontProvider(provider.GetProperty("file").GetString(),
                                    provider.GetProperty("ascent").GetInt32(),
                                    provider.GetProperty("chars").EnumerateArray().Select(p => p.GetString()),
                                    height.ValueKind == JsonValueKind.Number ? height.GetInt32() : 16)
                            );
                            break;
                        case "legacy_unicode":
                            var sizes = provider.GetProperty("sizes").GetString();
                            var template = provider.GetProperty("template").GetString();
                            _providers.Add(new UnicodeFontProvider(_assetProvider.GetFile(sizes), template));
                            break;
                        case "ttf":
                            Logger.GetLogger<Font>().Warn("Unsupported TrueType font provider.");
                            break;
                        default:
                            Logger.GetLogger<Font>().Warn($"Unknown font provider type: {type}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.GetLogger<Font>().Warn(ex);
                }
            }
        }

        public (NamedIdentifier file, float x1, float y1, float x2, float y2)? GetChar(char c)
        {
            return Providers.Select(p => p.GetChar(c)).FirstOrDefault(p => p != null);
        }
    }
}
