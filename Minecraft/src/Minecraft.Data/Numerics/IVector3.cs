using System;
using System.Collections;
using System.Collections.Generic;

namespace Minecraft.Data.Numerics
{
    public interface IVector3<T> : IList<T> where T : struct
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Z { get; set; }

        new T this[int index]
        {
            get =>
                index switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
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
                    case 2:
                        Z = value;
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
            yield return Z;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
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
            return X.Equals(item) || Y.Equals(item) || Z.Equals(item);
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

        public (T x, T y, T z) Deconstruct()
        {
            return (X, Y, Z);
        }

        void Deconstruct(out T x, out T y, out T z);
    }
    }