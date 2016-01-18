﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreCollection.Set.Infra
{
    internal class ListSet<T> : ILetterSimpleSet<T> where T : class
    {
        private T[] _Items;
        private int _Count = 0;

        internal ListSet()
        {
            _Items = new T[LetterSimpleSetFactory<T>.MaxList];
        }

        internal ListSet(T item)
        {
            _Items = new T[LetterSimpleSetFactory<T>.MaxList];
            _Items[0] = item;
            _Count = 1;
        }

        internal ListSet(HashSet<T> items)
        {
            int count = items.Count();
            if (count >= LetterSimpleSetFactory<T>.MaxList)
            {
                throw new ArgumentOutOfRangeException(
                                string.Format("items count ({0}) >= Max ({1})", count, LetterSimpleSetFactory<T>.MaxList));
            }

            _Items = new T[LetterSimpleSetFactory<T>.MaxList];

            int index = 0;
            foreach (T item in items)
            {
                _Items[index++] = item;
            }

            _Count = count;
        }

        private bool Add(T item)
        {
            if (Contains(item))
                return false;

            _Items[_Count++] = item;
            return true;
        }

        private bool Remove(T item)
        {
            if (item == null)
                return false;

            for (int i = 0; i < _Count; i++)
            {
                T iitem = _Items[i];
                if (Object.Equals(item, iitem))
                {
                    _Items[i] = _Items[_Count - 1];
                    _Items[_Count - 1] = null;
                    _Count--;
                    return true;
                }
            }

            return false;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _Count; i++)
            {
                if (Object.Equals( item, _Items[i]))
                    return true;
            }
            return false;
        }

        public int Count
        {
            get { return _Count; }
        }

        private IEnumerable<T> GetEnumerable()
        {
            return _Items.TakeWhile(t => t != null);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ILetterSimpleSet<T> Add(T item, out bool success)
        {
            success = Add(item);

            if (success && (_Count == _Items.Length))
            {
                return new SimpleHashSet<T>(GetEnumerable());
            }

            return this;
        }

        public ILetterSimpleSet<T> Remove(T item, out bool success)
        {
            success = Remove(item);
            return this;
        }
    }
}