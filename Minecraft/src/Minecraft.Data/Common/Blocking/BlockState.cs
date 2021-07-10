using System;
using System.Collections.Generic;
using System.Linq;
using Minecraft.Data.Nbt;
using Minecraft.Data.Nbt.Serialization;

namespace Minecraft.Data.Common.Blocking
{
    /// <summary>
    ///     一个方块状态。
    /// </summary>
    [NbtCompound]
    public class BlockState : ICloneable, IEquatable<BlockState>, IEquatable<string>
    {
        private string _name;
        private string _namespace;

        public BlockState()
        {
        }

        public BlockState(string @namespace, string name)
        {
            _namespace = @namespace;
            _name = name;
        }

        public BlockState(string @namespace, string name, IEnumerable<KeyValuePair<string, string>> properties)
        {
            _namespace = @namespace;
            _name = name;
            Properties = new Dictionary<string, string>(properties);
        }

        public BlockState(string name)
        {
            Name = name;
        }

        public BlockState(string name, IEnumerable<KeyValuePair<string, string>> properties)
        {
            Name = name;
            foreach (var kvp in properties)
                Properties.Add(kvp);
        }

        public string this[string propertyName]
        {
            get
            {
                if (!Properties.ContainsKey(propertyName))
                    return null;
                return Properties[propertyName];
            }
            set
            {
                if (!Properties.ContainsKey(propertyName))
                    Properties.Add(propertyName, value);
                Properties[propertyName] = value;
            }
        }

        /// <summary>
        ///     方块的命名空间ID。
        /// </summary>
        [NbtTag(Name = "Name", Type = NbtTagType.String)]
        public string Name
        {
            get => $"{_namespace ?? "minecraft"}:{_name}";
            set
            {
                var tmp = value.IndexOf(':');
                if (tmp == -1)
                {
                    _namespace = "minecraft";
                    _name = value;
                    return;
                }

                _namespace = value[..tmp];
                _name = value[(tmp + 1)..];
            }
        }

        /// <summary>
        ///     方块状态属性列表，Key代表的是方块状态属性的名称。
        /// </summary>
        [NbtTag(Name = "Properties", Type = NbtTagType.Compound)]
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        object ICloneable.Clone()
        {
            return Copy();
        }

        public bool Equals(BlockState other)
        {
            return other != null
                   && other.Name.Equals(Name)
                   && other.Properties
                       .OrderBy(kvp => kvp.Key)
                       .SequenceEqual(other.Properties.OrderBy(kvp => kvp.Key));
        }

        public bool Equals(string other)
        {
            return other != null && (other.Equals(Name) || other.Equals(_name));
        }

        public BlockState Copy()
        {
            return new BlockState
            {
                Name = Name,
                Properties = Properties
            };
        }

        public override bool Equals(object obj)
        {
            return obj is BlockState other && Equals(other)
                   || obj is string block && Equals(block);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Properties);
        }

        public static implicit operator BlockState(string name)
        {
            return new BlockState(name);
        }

        public static bool operator ==(BlockState left, string right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlockState left, string right)
        {
            return !left.Equals(right);
        }
    }
}