using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bifrost.Common.Core.ApplicationServices.Implementations
{
    /// <summary>
    /// Threadsafe cache with expire functionality
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TimedMemoryCache<TKey, TValue> : ICache<TKey, TValue>
        where TValue : class
    {
        private readonly TimeSpan _itemLongevity;
        private readonly object _locker = new object();
        private readonly IDictionary<TKey, TimedCacheItem<TValue>> _values;

        private Func<TKey, TValue> _onMissing = delegate(TKey key)
        {
            var message = string.Format("Key '{0}' could not be found", key);
            throw new KeyNotFoundException(message);
        };

        /// <summary>
        ///     Specifies how long each cache item should be valid for.
        ///     Expired cache items will not be retrieved and will count as a "no key found"
        /// </summary>
        /// <param name="itemLongevity"></param>
        public TimedMemoryCache(TimeSpan itemLongevity)
            : this(new Dictionary<TKey, TValue>(), null, itemLongevity)
        {
        }

        public TimedMemoryCache(TimeSpan itemLongevity, TimeSpan removalTimespan)
            : this(new Dictionary<TKey, TValue>(), null, itemLongevity, removalTimespan)
        {
        }

        public TimedMemoryCache(Func<TKey, TValue> onMissing, TimeSpan itemLongevity)
            : this(new Dictionary<TKey, TValue>(), onMissing, itemLongevity)
        {
        }

        public TimedMemoryCache(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> onMissing,
            TimeSpan itemLongevity)
            : this(dictionary, itemLongevity)
        {
            _onMissing = onMissing;
        }

        public TimedMemoryCache(IDictionary<TKey, TValue> dictionary, TimeSpan itemLongevity)

        {
            _itemLongevity = itemLongevity;
            _values = dictionary.ToDictionary(pair => pair.Key, pair => CreateTimedCacheItem(pair.Value));
        }

        /// <summary>
        /// </summary>
        public TimedMemoryCache(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, Func<TKey, TValue> onMissing,
            TimeSpan itemLongevity)
        {
            _itemLongevity = itemLongevity;
            _values = dictionary.ToDictionary(pair => pair.Key, pair => CreateTimedCacheItem(pair.Value));
            if (onMissing != null)
                _onMissing = onMissing;
        }

        /// <summary>
        /// </summary>
        /// <param name="itemLongevity"></param>
        /// <param name="removalTimespan">Define how often an event that removes the expired items will be run</param>
        public TimedMemoryCache(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, Func<TKey, TValue> onMissing,
            TimeSpan itemLongevity, TimeSpan removalTimespan)
            : this(dictionary, onMissing, itemLongevity)
        {
            RemovalTimer = new Timer(RemoveExpiredItems, null, new TimeSpan(0, 0, 0), removalTimespan);
        }

        public Timer RemovalTimer { get; }

        public Func<TKey, TValue> OnMissing
        {
            set => _onMissing = value;
        }

        public int Count
        {
            get { return _values.Count(x => !x.Value.HasExpired()); }
        }

        public TValue this[TKey key] => Retrieve(key);

        /// <summary>
        ///     Replaces the value of any existing key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Store(TKey key, TValue value)
        {
            if (_values.ContainsKey(key))
            {
                if (_values[key].HasExpired())
                    _values[key] = CreateTimedCacheItem(value);
            }
            else
            {
                _values.Add(key, CreateTimedCacheItem(value));
            }
        }

        /// <summary>
        ///     If the key already exists, do nothing
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Fill(TKey key, TValue value)
        {
            if (ContainsValidKey(key))
            {
                return;
            }
            Store(key, value);
        }

        /// <summary>
        ///     tries to retrieve the value of the given key
        ///     If the key doesen't exist, _onMissing will be called
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Retrieve(TKey key)
        {
            if (!ContainsValidKey(key))
            {
                lock (_locker)
                {
                    if (!ContainsValidKey(key))
                    {
                        var value = _onMissing(key);
                        Store(key, value);
                    }
                }
            }
            return _values[key].Item;
        }

        /// <summary>
        ///     Tries to retrieve the value of the given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryRetrieve(TKey key, out TValue value)
        {
            value = default(TValue);

            if (ContainsValidKey(key))
            {
                value = _values[key].Item;
                return true;
            }

            return false;
        }

        public bool Has(TKey key)
        {
            return ContainsValidKey(key);
        }

        public TKey[] GetAllKeys()
        {
            return _values.Keys.ToArray();
        }

        public TValue[] GetAll()
        {
            return _values.Values.Where(x => !x.HasExpired()).Select(x => x.Item).ToArray();
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

        private void RemoveExpiredItems(object ojbect)
        {
            lock (_locker)
            {
                foreach (var key in GetAllKeys().Where(key => _values[key].HasExpired()))
                {
                    Remove(key);
                }
            }
        }

        private bool ContainsValidKey(TKey key)
        {
            if (_values.ContainsKey(key))
            {
                return !_values[key].HasExpired();
            }
            return false;
        }

        private TimedCacheItem<TValue> CreateTimedCacheItem(TValue value)
        {
            return new TimedCacheItem<TValue>(value, DateTime.Now + _itemLongevity);
        }
    }
}