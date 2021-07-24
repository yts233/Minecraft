using System;
using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Data.Nbt.Tags
{
    public class NbtList : NbtTag, IEquatable<NbtList>
    {
        private readonly ICollection<NbtTag> _list = new List<NbtTag>();
        public override NbtTagType Type => NbtTagType.List;

        public override int Count => _list.Count;

        public override bool IsReadOnly => _list.IsReadOnly;

        public NbtTagType? ContentType => _list.FirstOrDefault()?.Type;

        public bool Equals(NbtList other)
        {
            return other != null && _list.Equals(other._list);
        }

        protected override IEnumerator<NbtTag> GetChildrenTags()
        {
            return _list.GetEnumerator();
        }

        protected override bool _Add(NbtTag item)
        {
            if (ContentType != null && item.Type != ContentType)
                throw new ArgumentException($"item should be {ContentType}", nameof(item));
            _list.Add(item);
            return true;
        }

        protected override void _Clear()
        {
            _list.Clear();
        }

        public override bool Contains(NbtTag item)
        {
            return _list.Contains(item);
        }

        public override void CopyTo(NbtTag[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public override bool Equals(NbtTag other)
        {
            return other is NbtList list && list.Equals(this);
        }

        public override int GetHashCode()
        {
            return _list != null ? _list.GetHashCode() : 0;
        }

        protected override bool _Remove(NbtTag item)
        {
            return _list.Remove(item);
        }
    }
}