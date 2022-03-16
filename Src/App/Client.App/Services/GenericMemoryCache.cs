

using Microsoft.Extensions.Caching.Memory;
using Client.App.Services.Interface;
using Client.App.Infrastructure.Extensions;
using Microsoft.Extensions.Primitives;

namespace Client.App.Services
{
    /// <summary>
    /// Thread safe memory cache for generic use - wraps IMemoryCache
    /// </summary>
    /// <typeparam name="TCacheItemData">Payload to store in the memory cache</typeparam>
    /// multiple paralell importing sessions</remarks>
    public class GenericMemoryCache: IGenericMemoryCache
    {
        private readonly string _prefixKey;
        private readonly int _defaultExpirationInSeconds;
        private static readonly object _locker = new object();


        public GenericMemoryCache(IMemoryCache memoryCache)
        {
              _prefixKey = "transaction";
            Cache = memoryCache;
            _defaultExpirationInSeconds = Math.Abs(1200); ;
        }

        /// <summary>
        /// Cache object if direct access is desired. Only allow exposing this for inherited types.
        /// </summary>
        protected IMemoryCache Cache { get; }

        public string PrefixKey(string key) => $"{_prefixKey}_{key}"; //to avoid IMemoryCache collisions with other parts of the same process, each cache key is always prefixed with a set prefix set by the constructor of this class.


        /// <summary>
        /// Adds an item to memory cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemToCache"></param>
        /// <returns></returns>
        public bool AddItem(string key, object itemToCache)
        {
            try
            {
                if (!key.StartsWith(_prefixKey))
                    key = PrefixKey(key);

                lock (_locker)
                {
                    if (!Cache.TryGetValue(key, out object existingItem))
                    {
                        var cts = new CancellationTokenSource(_defaultExpirationInSeconds > 0 ?
                            _defaultExpirationInSeconds * 1000 : -1);
                        var cacheEntryOptions = new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(cts.Token));

                        Cache.Set(key, itemToCache, cacheEntryOptions);
                        return true;
                    }
                }
                return false; //Item not added, the key already exists
            }
            catch (Exception err)
            {
             
                return false;
            }
        }

        public virtual List<T> GetValues<T>()
        {
            lock (_locker)
            {
                var values = Cache.GetValues<ICacheEntry>().Where(c => c.Value is T).Select(c => (T)c.Value).ToList();
                return values;
            }
        }



        /// <summary>
        /// Retrieves a cache item. Possible to set the expiration of the cache item in seconds. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetItem(string key)
        {
            try
            {
                if (!key.StartsWith(_prefixKey))
                    key = PrefixKey(key);
                lock (_locker)
                {
                    if (Cache.TryGetValue(key, out object cachedItem))
                    {
                        return cachedItem;
                    }
                }
                return default(object);

            }
            catch (Exception err)
            {
               
                return default(object);
            }
        }

        public bool SetItem(string key, object itemToCache)
        {
            try
            {
                if (!key.StartsWith(_prefixKey))
                    key = PrefixKey(key);
                lock (_locker)
                {
                    if (GetItem(key) != null)
                    {
                        AddItem(key, itemToCache);
                        return true;
                    }
                    UpdateItem(key, itemToCache);
                }
                return true;
            }
            catch (Exception err)
            {
               
                return false;
            }
        }


        /// <summary>
        /// Updates an item in the cache and set the expiration of the cache item 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemToCache"></param>
        /// <returns></returns>
        public bool UpdateItem(string key, object itemToCache)
        {
            if (!key.StartsWith(_prefixKey))
                key = PrefixKey(key);
            lock (_locker)
            {
                object existingItem = GetItem(key);
                if (existingItem != null)
                {
                    //always remove the item existing before updating
                    RemoveItem(key);
                }
                AddItem(key, itemToCache);
            }
            return true;

        }

        /// <summary>
        /// Removes an item from the cache 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveItem(string key)
        {
            if (!key.StartsWith(_prefixKey))
                key = PrefixKey(key);

            lock (_locker)
            {
                if (Cache.TryGetValue(key, out var item))
                {
                    if (item != null)
                    {

                    }
                    Cache.Remove(key);
                    return true;
                }
            }
            return false;
        }

        public void AddItems(Dictionary<string, object> itemsToCache)
        {
            foreach (var kvp in itemsToCache)
                AddItem(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Clear all cache keys starting with known prefix passed into the constructor.
        /// </summary>
        public void ClearAll()
        {
            lock (_locker)
            {
                List<string> cacheKeys = Cache.GetKeys<string>().Where(k => k.StartsWith(_prefixKey)).ToList();

                foreach (string cacheKey in cacheKeys)
                {
                    if (cacheKey.StartsWith(_prefixKey))
                        Cache.Remove(cacheKey);
                }
            }
        }


    }
}