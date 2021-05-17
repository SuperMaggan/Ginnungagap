using System;

namespace Bifrost.Common.Core.ApplicationServices
{
    public interface ICache<TKey, TValue> where TValue : class
    {
        /// <summary>
        ///     Invoked if a key doesen't exist in the cache
        /// </summary>
        Func<TKey, TValue> OnMissing { set; }

        /// <summary>
        ///     Number of existing cache items
        /// </summary>
        int Count { get; }

        TValue this[TKey key] { get; }

        /// <summary>
        ///     Replaces the value of any existing key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Store(TKey key, TValue value);

        /// <summary>
        ///     If the key already exists, do nothing
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Fill(TKey key, TValue value);

        /// <summary>
        ///     tries to retrieve the value of the given key
        ///     If the key doesen't exist, _onMissing will be called
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue Retrieve(TKey key);

        /// <summary>
        ///     Tries to retrieve the value of the given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryRetrieve(TKey key, out TValue value);

        /// <summary>
        ///     True if the given key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Has(TKey key);

        /// <summary>
        ///     Retrieves all existing keys
        /// </summary>
        /// <returns></returns>
        TKey[] GetAllKeys();

        /// <summary>
        ///     Retrieves all exisiting values
        /// </summary>
        /// <returns></returns>
        TValue[] GetAll();

        /// <summary>
        ///     Remove the key/value from the cache
        /// </summary>
        /// <param name="key"></param>
        void Remove(TKey key);

        /// <summary>
        ///     Delete all items
        /// </summary>
        void ClearAll();
    }
}