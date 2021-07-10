using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Minecraft.Resources
{
    /// <summary>
    ///     资源目录
    /// </summary>
    public class ResourceDirectory : Resource
    {
        private readonly List<Asset> _assets = new List<Asset>();

        private readonly ICollection<Language> _languages = new List<Language>();
        private IFilePath _filepath;

        /// <summary>
        ///     从真实存在的路径创建<see cref="ResourceDirectory" />
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public ResourceDirectory(string path, string name = null) : this(new FilePath(path), name)
        {
        }

        /// <summary>
        ///     从<see cref="IFilePath" />创建<see cref="ResourceDirectory" />
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public ResourceDirectory(IFilePath path, string name = null) : base(Uuid.NewUuid(), name ?? path.GetFileName(),
            null, path)
        {
        }

        public override int Count => _assets.Count;

        protected override void LoadAssets(object argument)
        {
            var root = _filepath = (IFilePath) argument;

            static IFilePath GetPackMcmeta(IFilePath rootPath)
            {
                IFilePath result;
                if ((result = rootPath["pack.mcmeta"]).IsFile) return result;
                if ((result = rootPath["assets"]["pack.mcmeta"]).IsFile) return result;
                throw new ResourceException("'pack.mcmeta' missing", new FileNotFoundException());
            }

            static dynamic GetPackInfo(IFilePath packMcmeta)
            {
                var jsonDocument = JsonDocument.Parse(packMcmeta.OpenRead());
                var root = jsonDocument.RootElement;
                var pack = root.GetProperty("pack");
                var format = pack.GetProperty("pack_format").GetInt32();
                var description = pack.GetProperty("description").GetString();
                var languages = new List<dynamic>();
                foreach (var lang in root.GetProperty("language").EnumerateObject())
                    languages.Add(new
                    {
                        id = lang.Name,
                        name = lang.Value.GetProperty("name").GetString(),
                        region = lang.Value.GetProperty("region").GetString(),
                        bidirectional = lang.Value.GetProperty("bidirectional").GetBoolean()
                    });
                return new {format, description, languages};
            }

            static IEnumerable<Asset> LoadAsset(AssetType type, Resource resource, IFilePath pathBase,
                string @namespace, IFilePath currentPath = null)
            {
                currentPath ??= pathBase;
                if (!currentPath.Exists) yield break;
                if (currentPath.IsDirectory)
                    foreach (var subPath in currentPath)
                    foreach (var asset in LoadAsset(type, resource, pathBase, @namespace, subPath))
                        yield return asset;
                else if (currentPath.IsFile)
                    yield return new AssetFile(
                        string.Join(
                            '/',
                            currentPath.GetUpDirectories()
                                .Where(path => !Equals(path, pathBase))
                                .Where(path => !pathBase.GetUpDirectories().Contains(path))
                                .Append(currentPath)
                                .OrderBy(path => path.PathName.Length)
                                .Select(path => path.GetFileName())),
                        type,
                        resource,
                        currentPath,
                        @namespace);
            }

            var info = GetPackInfo(GetPackMcmeta(root));
            foreach (var directory in root["assets"].GetDirectories())
            {
                var @namespace = directory.GetFileName();

                void LoadFolder(string dir, AssetType assetType)
                {
                    var path = directory[dir];
                    if (path.Exists)
                        _assets.AddRange(LoadAsset(assetType, this, path, @namespace));
                }

                // ReSharper disable once StringLiteralTypo
                LoadFolder("blockstates", AssetType.Blockstate);
                LoadFolder("font", AssetType.Font);
                LoadFolder("icons", AssetType.Icon);
                LoadFolder("lang", AssetType.Lang);
                LoadFolder("models", AssetType.Model);
                LoadFolder("shaders", AssetType.Shader);
                LoadFolder("sounds", AssetType.Sound);
                LoadFolder("texts", AssetType.Text);
                LoadFolder("textures", AssetType.Texture);
            }

            Description = info.description;
            foreach (var languageInfo in info.languages)
                _languages.Add(new Language(
                    languageInfo.id,
                    languageInfo.region,
                    languageInfo.name,
                    languageInfo.bidirectional,
                    _assets
                        .Where(asset => asset.Type == AssetType.Lang)
                        .Where(asset => asset.Name.StartsWith(languageInfo.id))
                        .ToList()
                ));
        }

        public override IEnumerable<Asset> GetAssets()
        {
            return _assets;
        }

        public override IEnumerable<Language> GetLanguages()
        {
            return _languages;
        }

        public override void Dispose()
        {
            _filepath.Dispose();
        }
    }
}