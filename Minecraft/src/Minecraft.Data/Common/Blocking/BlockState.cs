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
    public class BlockState : ICloneable, IEquatable<BlockState>,IEquatable<NamedIdentifier>
    {
        public BlockState()
        {
        }

        public BlockState(NamedIdentifier id)
        {
            Name = id;
        }

        public BlockState(NamedIdentifier id, IEnumerable<KeyValuePair<string, string>> properties)
        {
            Name = id;
            Properties = new Dictionary<string, string>(properties);
        }

        public string this[string propertyName]
        {
            get => !Properties.ContainsKey(propertyName) ? null : Properties[propertyName];
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
        public NamedIdentifier Name { get; set; }

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

        public bool Equals(NamedIdentifier other)
        {
            return other != null && Name.Equals(other);
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
                   || obj is NamedIdentifier id && Equals(id);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return HashCode.Combine(Name,
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                Properties);
        }

        public static implicit operator BlockState(string name)
        {
            return new BlockState(name);
        }

        public static implicit operator BlockState(NamedIdentifier name)
        {
            return new BlockState(name);
        }

        public static bool operator ==(BlockState left, NamedIdentifier right)
        {
            return left != null && left.Equals(right);
        }

        public static bool operator !=(BlockState left, NamedIdentifier right)
        {
            return left != null && !left.Equals(right);
        }
    }
}