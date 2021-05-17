using System;
using System.Collections.Generic;
using System.Linq;

namespace Bifrost.Common.Core.ApplicationServices.Implementations
{
    public class MemoryCache<TKey, TValue> : ICache<TKey, TValue> where TValue : class
    {
        private readonly object _locker = new object();
        private readonly IDictionary<TKey, TValue> _values;

        private Func<TKey, TValue> _onMissing = delegate(TKey key)
        {
            var message = string.Format("Key '{0}' could not be found", key);
            throw new KeyNotFoundException(message);
        };

        public MemoryCache()
            : this(new Dictionary<TKey, TValue>())
        {
        }

        public MemoryCache(Func<TKey, TValue> onMissing)
            : this(new Dictionary<TKey, TValue>(), onMissing)
        {
        }

        public MemoryCache(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> onMissing)
            : this(dictionary)
        {
            _onMissing = onMissing;
        }

        public MemoryCache(IDictionary<TKey, TValue> dictionary)
        {
            _values = dictionary;
        }

        public Func<TKey, TValue> OnMissing
        {
            set { _onMissing = value; }
        }

        public int Count
        {
            get { return _values.Count; }
        }

        public TValue this[TKey key]
        {
            get { return Retrieve(key); }
        }

        public void Store(TKey key, TValue value)
        {
            if (_values.ContainsKey(key))
            {
                _values[key] = value;
            }
            else
            {
                _values.Add(key, value);
            }
        }

        public void Fill(TKey key, TValue value)
        {
            if (_values.ContainsKey(key))
            {
                return;
            }

            _values.Add(key, value);
        }

        public TValue Retrieve(TKey key)
        {
            if (!_values.ContainsKey(key))
            {
                lock (_locker)
                {
                    if (!_values.ContainsKey(key))
                    {
                        var value = _onMissing(key);
                        _values.Add(key, value);
                    }
                }
            }

            return _values[key];
        }

        public bool TryRetrieve(TKey key, out TValue value)
        {
            value = default(TValue);

            if (_values.ContainsKey(key))
            {
                value = _values[key];
                return true;
            }

            return false;
        }

        public bool Has(TKey key)
        {
            return _values.ContainsKey(key);
        }

        public TKey[] GetAllKeys()
        {
            return _values.Keys.ToArray();
        }

        public TValue[] GetAll()
        {
            return _values.Values.ToArray();
        }

        public void Remove(TKey key)
        {
            if (_values.ContainsKey(key))
            {
                _values.Remove(key);
            }
        }

        public void ClearAll()
        {
            _values.Clear();
        }
    }
}