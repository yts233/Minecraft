using System;
using System.Text.RegularExpressions;

namespace Minecraft
{
    /// <summary>
    /// 命名标识符
    /// </summary>
    /// <remarks>形如 minecraft:thing 的字符id</remarks>
    public class NamedIdentifier : IEquatable<NamedIdentifier>, IEquatable<string>
    {
        /// <summary>
        /// 创建一个<see cref="NamedIdentifier"/>
        /// </summary>
        /// <param name="fullName">完整名称（自动补全命名）</param>
        public NamedIdentifier(string fullName)
        {
            var index = fullName.IndexOf(':');
            if (index == -1)
            {
                Namespace = "minecraft";
                Name = fullName;
            }
            else if (index == fullName.Length - 1)
            {
                Namespace = fullName[..index];
                Name = "";
            }
            else
            {
                Namespace = fullName[..index];
                Name = fullName[(index + 1)..];
            }
        }

        public NamedIdentifier(string @namespace, string name)
        {
            Namespace = @namespace;
            Name = name;
        }

        public string Namespace { get; }
        public string Name { get; }
        public string FullName => $"{Namespace}:{Name}";

        private static readonly Regex NamespaceRegex = new Regex("^[0123456789abcdefghijklmnopqrstuvwxyz_\\-.]+$");
        private static readonly Regex NameRegex = new Regex("^[0123456789abcdefghijklmnopqrstuvwxyz_\\-.\\/]+$");
        public bool IsValid => NamespaceRegex.IsMatch(Namespace) && NameRegex.IsMatch(Name);

        public override string ToString()
        {
            return FullName;
        }

        public override bool Equals(object obj)
        {
            return obj is NamedIdentifier id && Equals(id)
                   || obj is string s && Equals(s);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Namespace, Name);
        }

        public bool Equals(NamedIdentifier other)
        {
            return other != null && Namespace == other.Namespace && Name == other.Name;
        }

        public bool Equals(string other)
        {
            return other != null && (FullName == other || Namespace == "minecraft" && Name == other);
        }
        
        public static implicit operator NamedIdentifier(string id)
        {
            return new NamedIdentifier(id);
        }
    }
}