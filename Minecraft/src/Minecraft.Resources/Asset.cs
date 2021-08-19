using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Minecraft.Resources
{
    /// <summary>
    /// 资源内容
    /// </summary>
    public abstract class Asset : IEquatable<Asset>, IDisposable, INamedObject, IIdentify
    {
        /// <summary>
        /// 创建一个<see cref="Asset" />
        /// </summary>
        /// <param name="type"><see cref="Asset" />的类型</param>
        /// <param name="resource"><see cref="Asset" />的所属资源</param>
        /// <param name="uuid"><see cref="Asset" />的标识符</param>
        /// <exception cref="ResourceException">命名空间或名字格式错误</exception>
        protected Asset(NamedIdentifier namedId, AssetType type, Resource resource, Uuid uuid)
        {
            if (!namedId.IsValid)
                throw new ResourceException("invalid namespace or name");
            NamedIdentifier = namedId;
            Type = type;
            Resource = resource;
            Id = uuid;
        }

        /// <summary>
        /// 命名Id
        /// </summary>
        public NamedIdentifier NamedIdentifier { get; }

        /// <summary>
        /// 类型
        /// </summary>
        public AssetType Type { get; }

        /// <summary>
        /// 所属资源
        /// </summary>
        public Resource Resource { get; }

        /// <summary>
        /// 释放资源
        /// </summary>
        public abstract void Dispose();

        bool IEquatable<Asset>.Equals(Asset other)
        {
            return Id == other?.Id;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Uuid Id { get; }

        /// <summary>
        /// 打开数据流
        /// </summary>
        /// <returns></returns>
        public abstract Stream OpenRead();

        /// <summary>
        /// 打开文本流
        /// </summary>
        /// <returns></returns>
        public TextReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        public override bool Equals(object obj)
        {
            return obj is Asset ass && Id.Equals(ass.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Type}: {NamedIdentifier}";
        }
    }
}