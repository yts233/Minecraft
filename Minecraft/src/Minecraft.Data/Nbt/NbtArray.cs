using System;
using System.Collections.Generic;
using System.Data;

namespace Minecraft.Data.Nbt
{
    public abstract class NbtArray<T> : NbtTag, IList<T>, IEquatable<NbtArray<T>>
    {
        private readonly IList<T> _value = new List<T>();

        protected bool ArrayReadOnly { get; set; }

        public bool Equals(NbtArray<T> other)
        {
            return other != null && _value.Equals(other._value);
        }

        public override int Count => _value.Count;

        public override bool IsReadOnly => ArrayReadOnly;

        public IEnumerator<T> GetEnumerator()
        {
            return _value.GetEnumerator();
        }

        public void Add(T item)
        {
            if (IsReadOnly)
                throw new ReadOnlyException();
            _value.Add(item);
        }

        public bool Contains(T item)
        {
            return _value.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _value.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return !IsReadOnly && _value.Remove(item);
        }

        public int IndexOf(T item)
        {
            return _value.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (IsReadOnly)
                throw new ReadOnlyException();
            _value.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly)
                throw new ReadOnlyException();
            _value.RemoveAt(index);
        }

        public T this[int index]
        {
            get => _value[index];
            set => _value[index] = value;
        }

        private static T EnsureValue(NbtTag item)
        {
            if (!(item is NbtValue value))
                throw new InvalidOperationException($"Can not add/remove {item.GetType()} of {typeof(T)} array.");
            if (!(value.Value is T returnValue))
                throw new InvalidOperationException(
                    $"Can not add/remove {value.Value.GetType()} of {typeof(T)} array.");
            return returnValue;
        }

        protected override IEnumerator<NbtTag> GetChildrenTags()
        {
            yield break;
        }

        protected override bool _Add(NbtTag item)
        {
            if (IsReadOnly)
                return false;
            _value.Add(EnsureValue(item));
            return true;
        }

        protected override void _Clear()
        {
            if (IsReadOnly)
                throw new ReadOnlyException();
            _value.Clear();
        }

        protected override bool _Remove(NbtTag item)
        {
            if (IsReadOnly)
                return false;
            _value.Add(EnsureValue(item));
            return true;
        }

        public override bool Contains(NbtTag item)
        {
            return _value.Contains(EnsureValue(item));
        }

        public override void CopyTo(NbtTag[] array, int arrayIndex)
        {
        }

        public override bool Equals(NbtTag other)
        {
            return other is NbtArray<T> array && Equals(array);
        }

        public override int GetHashCode()
        {
            return _value != null ? _value.GetHashCode() : 0;
        }
    }
}