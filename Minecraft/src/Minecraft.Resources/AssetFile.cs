using System.Collections.Generic;
using System.IO;

namespace Minecraft.Resources
{
    /// <summary>
    ///     Asset文件
    /// </summary>
    public class AssetFile : Asset
    {
        private readonly IFilePath _file;
        private readonly ICollection<Stream> _openedStream = new List<Stream>();

        /// <summary>
        ///     创建<see cref="AssetFile" />
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type">类型</param>
        /// <param name="resource">所属资源</param>
        /// <param name="id">标识符</param>
        /// <param name="file">文件</param>
        /// <param name="namespace">命名空间</param>
        public AssetFile(string name, AssetType type, Resource resource, Uuid id, IFilePath file,
            string @namespace = "minecraft") :
            base(name, type, resource, id, @namespace)
        {
            _file = file;
        }

        /// <summary>
        ///     创建<see cref="AssetFile" />
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type">类型</param>
        /// <param name="resource">所属资源</param>
        /// <param name="file">文件</param>
        /// <param name="namespace">命名空间</param>
        public AssetFile(string name, AssetType type, Resource resource, IFilePath file,
            string @namespace = "minecraft")
            : this(name, type, resource, Uuid.NewUuid(), file, @namespace)
        {
        }

        public override Stream OpenRead()
        {
            var tmp = _file.OpenRead();
            _openedStream.Add(tmp);
            return tmp;
        }

        public override void Dispose()
        {
            foreach (var fs in _openedStream)
                fs.Dispose();
        }
    }
}