﻿using System;
using System.Collections.Generic;

namespace MoreCollection.Extensions
{
    public static class DictionaryExtension

    {
        public static CollectionResult<TValue> GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));

            var res = default(TValue);
            if (dic.TryGetValue(key, out res))
                return new CollectionResult<TValue>() { Item = res, CollectionStatus = CollectionStatus.Found };

            res = Fac(key);
            dic.Add(key, res);
            return new CollectionResult<TValue>() { Item = res, CollectionStatus = CollectionStatus.Created };
        }

        public static TValue GetOrAddEntity<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));

            var res = default(TValue);
            if (dic.TryGetValue(key, out res))
                return res;

            res = Fac(key);
            dic.Add(key, res);
            return res;
        }

        public static TValue UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key,  Func<TKey, TValue> Fac,
                                                                Func<TKey, TValue, TValue> Updater)
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));

            var res = default(TValue);
            if (dic.TryGetValue(key, out res))
            {
                TValue nv = Updater(key, res);
                dic[key] = nv;
                return nv;
            }

            res = Fac(key);
            dic.Add(key, res);
            return res;
        }

        public static TValue UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac,
                                                                Action<TKey, TValue> Updater)
        {
            return dic.UpdateOrAdd(key, Fac, (k, v) => { Updater(k, v); return v; });
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue= default(TValue))
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));

            TValue res;
            if (dic.TryGetValue(key, out res))
                return res;

            return defaultValue;
        }

        public static IDictionary<TKey, TValue> Import<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> source)
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));

            source.ForEach(el => dic.Add(el.Key, el.Value));
            return dic;
        }
    }
}
