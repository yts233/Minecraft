using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minecraft.Resources
{
    public class ResourceManager
    {
        private static readonly Logger<ResourceManager> _logger = Logger.GetLogger<ResourceManager>();

        private readonly Func<IEnumerable<Resource>> _resourceProvider;
        private readonly Action<IEnumerable<Resource>> _resourceDisposer;

        public IEnumerable<Resource> ActiveResources { get; private set; } = new List<Resource>();
        public IEnumerable<Asset> ActiveAssets { get; private set; } = new List<Asset>();


        public ResourceManager(Func<IEnumerable<Resource>> resourceProvider, Action<IEnumerable<Resource>> resourceDisposer)
        {
            _resourceProvider = resourceProvider;
            _resourceDisposer = resourceDisposer;
        }

        private class AssetNameEqualityComparer : IEqualityComparer<Asset>
        {
            public bool Equals(Asset x, Asset y)
            {
                return x.NamedIdentifier.Equals(y.NamedIdentifier);
            }

            public int GetHashCode(Asset obj)
            {
                return obj.NamedIdentifier.GetHashCode();
            }
        }

        private static readonly AssetNameEqualityComparer AssetComparer = new ();

        public void Reload()
        {
            _resourceDisposer(ActiveResources);
            ActiveResources = _resourceProvider().ToArray();
            ActiveAssets = ActiveResources.SelectMany(r => r.GetAssets()).Distinct(AssetComparer);
            _logger.Info($"Reload resource manager: {string.Join(", ", ActiveResources.Select(r => r.Name))}");
        }

        public bool TryGetAsset(AssetType type, NamedIdentifier name, out Asset asset)
        {
            asset = ActiveAssets.FirstOrDefault(a => a.Type == type && a.NamedIdentifier == name);
            return asset != null;
        }
    }
}
