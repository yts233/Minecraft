using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Data.Nbt.Tags
{
    public class NbtCompound : NbtTag, IReadOnlyDictionary<string, NbtTag>
    {
        private readonly List<NbtTag> _values = new List<NbtTag>();

        public override NbtTagType Type => NbtTagType.Compound;

        public override bool IsReadOnly => false;

        public override int Count => _values.Count;

        public IEnumerator<KeyValuePair<string, NbtTag>> GetEnumerator()
        {
            return _values.Select(tag => new KeyValuePair<string, NbtTag>(tag.Name, tag)).GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return _values.Any(tag => tag.Name == key);
        }

        public bool TryGetValue(string key, out NbtTag value)
        {
            return (value = _values.FirstOrDefault(tag => tag.Name == key)) != null;
        }

        public NbtTag this[string key] => _values.FirstOrDefault(tag => tag.Name == key);

        public IEnumerable<string> Keys => _values.Select(tag => tag.Name);

        public IEnumerable<NbtTag> Values => _values;

        protected override IEnumerator<NbtTag> GetChildrenTags()
        {
            return _values.GetEnumerator();
        }

        protected override bool _Add(NbtTag item)
        {
            _values.Add(item);
            return true;
        }

        protected override void _Clear()
        {
            _values.Clear();
        }

        public override bool Contains(NbtTag item)
        {
            return _values.Contains(item);
        }

        public override void CopyTo(NbtTag[] array, int arrayIndex)
        {
            _values.CopyTo(array, arrayIndex);
        }

        public override bool Equals(NbtTag other)
        {
            return other is NbtCompound compound && compound._values.SequenceEqual(_values);
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        protected override bool _Remove(NbtTag item)
        {
            return _values.Remove(item);
        }
    }
}