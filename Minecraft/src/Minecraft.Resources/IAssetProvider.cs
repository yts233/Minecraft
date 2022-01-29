using System.Linq;
using System.Collections.Generic;

namespace Minecraft.Resources
{
    public interface IAssetProvider
    {
        Asset this[AssetType type, NamedIdentifier name] { get; }

        bool TryGetAsset(AssetType type, NamedIdentifier name, out Asset asset);

        IEnumerable<Asset> GetAssets();
    }
}