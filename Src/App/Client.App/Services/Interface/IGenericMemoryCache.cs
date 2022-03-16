

namespace Client.App.Services.Interface
{
    public interface IGenericMemoryCache
    {
        string PrefixKey(string key);

        /// <summary>
        /// Adds an item to memory cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemToCache"></param>
        /// <returns></returns>
        bool AddItem(string key, object itemToCache);

        List<T> GetValues<T>();

        /// <summary>
        /// Retrieves a cache item. Possible to set the expiration of the cache item in seconds. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetItem(string key);

        bool SetItem(string key, object itemToCache);

        /// <summary>
        /// Updates an item in the cache and set the expiration of the cache item 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemToCache"></param>
        /// <returns></returns>
        bool UpdateItem(string key, object itemToCache);

        /// <summary>
        /// Removes an item from the cache 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool RemoveItem(string key);

        void AddItems(Dictionary<string, object> itemsToCache);

        /// <summary>
        /// Clear all cache keys starting with known prefix passed into the constructor.
        /// </summary>
        void ClearAll();
    }
}
