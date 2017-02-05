﻿using MoreCollection.Dictionary.Internal;
using MoreCollection.Dictionary.Internal.Strategy;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MoreCollection.Dictionary
{
    [DebuggerDisplay("Count = {Count}")]
    public class HybridDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private IMutableDictionary<TKey, TValue> _Implementation;

        public HybridDictionary(int ExceptedCapacity=0, int TransitionToDictionary = 25)
        {
            var strategy = DictionaryStrategyFactory.GetStrategy<TKey, TValue>(TransitionToDictionary);
            _Implementation = strategy.GetEmpty(ExceptedCapacity);       
        }

        public void Add(TKey key, TValue value)
        {
            _Implementation = _Implementation.AddMutable(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _Implementation = _Implementation.ClearMutable();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue tv;

            if (!_Implementation.TryGetValue(item.Key, out tv) || (!Object.Equals(tv, item.Value)))
                return false;

            return Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            bool res;
            _Implementation = _Implementation.Remove(key, out res);
            return res;
        }

        public bool ContainsKey(TKey key)
        {
            return  _Implementation.ContainsKey(key);
        }

        public ICollection<TKey> Keys => _Implementation.Keys;

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _Implementation.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values => _Implementation.Values;

        public TValue this[TKey key]
        {
            get { return _Implementation[key]; }
            set { _Implementation = _Implementation.Update(key, value); }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _Implementation.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();

            _Implementation.CopyTo(array, arrayIndex);
        }

        public int Count => _Implementation.Count;

        public bool IsReadOnly => false;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _Implementation.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
