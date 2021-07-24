using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Minecraft.Data.Nbt
{
    public abstract class NbtArray<T> : NbtTag, IList<T>, IEquatable<NbtArray<T>>
    {
        private readonly T[] _value;

        public T[] Value => _value;

        public NbtArray(T[] value)
        {
            _value = value;
        }

        protected bool ArrayReadOnly { get; set; }

        public bool Equals(NbtArray<T> other)
        {
            return other != null && _value.Equals(other._value);
        }

        public override int Count => _value.Length;

        public override bool IsReadOnly => ArrayReadOnly;

        public IEnumerator<T> GetEnumerator()
        {
            return _value.Cast<T>().GetEnumerator();
        }

        public void Add(T item)
        {
            throw new ReadOnlyException();
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
            return false;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_value, item);
        }

        public void Insert(int index, T item)
        {
            throw new ReadOnlyException();
        }

        public void RemoveAt(int index)
        {
            throw new ReadOnlyException();
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
            return false;
        }

        protected override void _Clear()
        {
            throw new ReadOnlyException();
        }

        protected override bool _Remove(NbtTag item)
        {
            return false;
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