//from https://www.codeproject.com/Tips/5260474/Efficiently-Using-Lists-as-Dictionary-Keys-in-Csha
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace myotui.Models
{
    public sealed class KeyList<T> : IReadOnlyList<T>, IEquatable<KeyList<T>>
    {
        int _hashCode;
        List<T> _items;
        public KeyList()
        {
            _items = new List<T>();
            _hashCode = 0;
        }
        public KeyList(int capacity)
        {
            _items = new List<T>(capacity);
            _hashCode = 0;
        }

        public KeyList(IList<T> list)
        {
            _items = new List<T>();
            _hashCode = 0;
            list.ToList().ForEach(element => Add(element));
        }
        public T this[int index] => _items[index];

        public int Count => _items.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void Add(T item)
        {
            _items.Add(item);
            if (null != item)
                _hashCode ^= item.GetHashCode();
        }
        public override int GetHashCode()
        {
            return _hashCode;
        }
        public bool Equals(KeyList<T> rhs)
        {
            if (ReferenceEquals(this, rhs))
                return true;
            if (ReferenceEquals(rhs, null))
                return false;
            if (rhs._hashCode != _hashCode)
                return false;
            var ic = _items.Count;
            if (ic != rhs._items.Count)
                return false;
            for(var i = 0;i<ic;++i)
                if (!Equals(_items[i], rhs._items[i]))
                    return false;
            return true;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as KeyList<T>);
        }
    }
}