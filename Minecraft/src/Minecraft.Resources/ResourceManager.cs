using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minecraft.Resources
{
    public class ResourceManager : IAssetProvider
    {
        private static readonly Logger<ResourceManager> _logger = Logger.GetLogger<ResourceManager>();

        private readonly Func<IEnumerable<Resource>> _resourceProvider;
        private readonly Action<IEnumerable<Resource>> _resourceDisposer;

        public IEnumerable<Resource> ActiveResources { get; private set; } = new List<Resource>();
        public IReadOnlyDictionary<KeyValuePair<AssetType, NamedIdentifier>, Asset> ActiveAssets { get; private set; } = new Dictionary<KeyValuePair<AssetType, NamedIdentifier>, Asset>();
        public Asset this[AssetType type, NamedIdentifier name] { get => ActiveAssets[new KeyValuePair<AssetType, NamedIdentifier>(type, name)]; }


        public ResourceManager(Func<IEnumerable<Resource>> resourceProvider, Action<IEnumerable<Resource>> resourceDisposer)
        {
            _resourceProvider = resourceProvider;
            _resourceDisposer = resourceDisposer;
        }

        private class AssetNameEqualityComparer : IEqualityComparer<Asset>
        {
            public bool Equals(Asset x, Asset y)
            {
                return x.NamedIdentifier.Equals(y.NamedIdentifier) && x.Type.Equals(y.Type);
            }

            public int GetHashCode(Asset obj)
            {
                return HashCode.Combine(obj.NamedIdentifier.GetHashCode(), obj.Type.GetHashCode());
            }
        }

        private static readonly AssetNameEqualityComparer AssetComparer = new();

        public void Reload()
        {
            _resourceDisposer(ActiveResources);
            ActiveResources = _resourceProvider().ToArray();
            ActiveAssets = ActiveResources.SelectMany(r => r.GetAssets()).Distinct(AssetComparer).ToDictionary(s => new KeyValuePair<AssetType, NamedIdentifier>(s.Type, s.NamedIdentifier));
            _logger.Info($"Reload resource manager: {string.Join(", ", ActiveResources.Select(r => r.Name))}");
        }

        public bool TryGetAsset(AssetType type, NamedIdentifier name, out Asset asset)
        {
            ActiveAssets.TryGetValue(new KeyValuePair<AssetType, NamedIdentifier>(type, name), out asset);
            return asset != null;
        }

        public IEnumerable<Asset> GetAssets()
        {
            return ActiveAssets.Values;
        }
    }
}
