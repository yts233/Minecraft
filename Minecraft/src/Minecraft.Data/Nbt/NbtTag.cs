using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Data.Nbt
{
    public abstract class NbtTag : ICollection<NbtTag>, IEquatable<NbtTag>
    {
        public virtual NbtTag this[object key]
        {
            get
            {
                return key switch
                {
                    int index => this.Skip(index).FirstOrDefault(),
                    string tagName => this.FirstOrDefault(tag => tag.Name == tagName),
                    _ => null
                };
            }
        }

        public NbtTag Parent { get; private set; }

        public NbtTag Root
        {
            get
            {
                var current = this;
                for (; current.Parent != null; current = current.Parent)
                {
                }

                return current;
            }
        }

        public string Name { get; set; }

        public abstract NbtTagType Type { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetChildrenTags();
        }

        IEnumerator<NbtTag> IEnumerable<NbtTag>.GetEnumerator()
        {
            return GetChildrenTags();
        }

        public void Add(NbtTag item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (!_Add(item)) return;

            item.Parent?.__Remove(item);
            item.Parent = this;
        }

        public void Clear()
        {
            foreach (var tag in this)
                tag.Parent = null;
            _Clear();
        }

        public abstract bool Contains(NbtTag item);

        public abstract void CopyTo(NbtTag[] array, int arrayIndex);

        public bool Remove(NbtTag item)
        {
            if (item == null) return false;
            if (!_Remove(item)) return false;
            item.Parent = null;
            return true;
        }

        public abstract int Count { get; }

        public abstract bool IsReadOnly { get; }

        public abstract bool Equals(NbtTag other);

        protected abstract IEnumerator<NbtTag> GetChildrenTags();

        protected abstract bool _Add(NbtTag item);

        public void Add(NbtTag item, string name)
        {
            item.Name = name;
            Add(item);
        }

        protected abstract void _Clear();

        protected abstract bool _Remove(NbtTag item);

        private void __Remove(NbtTag item)
        {
            _Remove(item);
        }

        public IEnumerable<NbtTag> GetAllTags()
        {
            var list = new List<NbtTag>();

            void CheckChildren(NbtTag tag)
            {
                list.Add(tag);
                foreach (var nbtTag in tag)
                    CheckChildren(nbtTag);
            }

            CheckChildren(this);
            return list;
        }

        public override bool Equals(object obj)
        {
            return obj is NbtTag other && other.Equals(this);
        }

        public abstract override int GetHashCode();

        public override string ToString()
        {
            return this.ToStringedNbt(true);
        }
    }
}