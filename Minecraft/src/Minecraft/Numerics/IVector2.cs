using System;
using System.Collections;
using System.Collections.Generic;

namespace Minecraft.Numerics
{
    public interface IVector2<T> : IList<T> where T : struct
    {
        public T X { get; set; }
        public T Y { get; set; }

        new T this[int index]
        {
            get =>
                index switch
                {
                    0 => X,
                    1 => Y,
                    _ => throw new ArgumentOutOfRangeException(nameof(index))
                };
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            yield return X;
            yield return Y;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return X;
            yield return Y;
        }

        void ICollection<T>.Add(T item)
        {
            throw new InvalidOperationException();
        }

        void ICollection<T>.Clear()
        {
            throw new InvalidOperationException();
        }

        bool ICollection<T>.Contains(T item)
        {
            return X.Equals(item) || Y.Equals(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new InvalidOperationException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new InvalidOperationException();
        }

        int ICollection<T>.Count => 3;

        bool ICollection<T>.IsReadOnly => false;

        int IList<T>.IndexOf(T item)
        {
            throw new InvalidOperationException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new InvalidOperationException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        public (T x, T y) Deconstruct()
        {
            return (X, Y);
        }

        void Deconstruct(out T x, out T y);

        void Add(IVector2<T> other);
        void Delta(IVector2<T> other);
        void Scale(T other);

        T LengthPow2 { get; }
        T Length { get; }
    }
}