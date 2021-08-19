using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace Minecraft.Resources
{
    /// <summary>
    /// 语言
    /// </summary>
    public class Language : IEnumerable<Translation>
    {
        private readonly IEnumerable<Asset> _langFiles;
        private readonly ICollection<Translation> _translations = new List<Translation>();
        private bool _loaded;

        /// <summary>
        /// 创建语言
        /// </summary>
        /// <param name="id">标识符</param>
        /// <param name="region">区域</param>
        /// <param name="name">名称</param>
        /// <param name="bidirectional">从右值左</param>
        /// <param name="langFiles">语言文件</param>
        public Language(string id, string region, string name, bool bidirectional,
            IEnumerable<Asset> langFiles)
        {
            Id = id;
            Region = region;
            Name = name;
            Bidirectional = bidirectional;
            _langFiles = langFiles;
        }

        /// <summary>
        /// 标识符
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 从右至左
        /// </summary>
        public bool Bidirectional { get; }

        IEnumerator<Translation> IEnumerable<Translation>.GetEnumerator()
        {
            return _translations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _translations.GetEnumerator();
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <exception cref="ResourceException">语言文件格式错误</exception>
        public void Load()
        {
            if (_loaded) return;
            foreach (var asset in _langFiles)
            {
                var reader = asset.OpenText();
                var @namespace = ((INamedObject)asset).Namespace;
                var isJson = false;
                while (true)
                {
                    var @char = reader.Read();
                    if (@char == -1) break;
                    switch (@char)
                    {
                        case ' ':
                        case '\n':
                        case '\r':
                            continue;
                    }

                    isJson = @char == '{';
                    break;
                }

                reader.Dispose();
                if (isJson)
                {
                    var jsonDocument = JsonDocument.Parse(asset.OpenRead());
                    foreach (var translation in jsonDocument.RootElement.EnumerateObject())
                        _translations.Add(new Translation((@namespace, translation.Name), translation.Value.GetString()));
                }
                else
                {
                    reader = asset.OpenText();
                    string line;
                    var lines = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines++;
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var index = line.IndexOf('=');
                        if (index == -1)
                            throw new ResourceException(
                                $"format incorrect at line {lines} in lang file {asset.NamedIdentifier}.");
                        var name = line[..index];
                        var value = index + 1 == line.Length ? "" : line[(index + 1)..];
                        _translations.Add(new Translation((@namespace, name), value));
                    }
                }
            }

            _loaded = true;
        }
    }
}