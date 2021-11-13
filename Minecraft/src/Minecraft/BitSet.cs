using System.Collections;
using System.Collections.Generic;

namespace Minecraft
{
    public class BitSet : IList<bool>
    {
        private readonly List<long> _data = new List<long>();

        public bool this[int index] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public void Add(bool item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(bool item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(bool[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<bool> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public int IndexOf(bool item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(int index, bool item)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(bool item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}