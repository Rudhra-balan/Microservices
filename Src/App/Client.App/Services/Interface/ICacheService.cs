
namespace Client.App.Services.Interface
{
    public interface ICacheService
    {
        T GetOrAdd<T>(string cacheKey, Func<T> factory);
        Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> actionMethod);
    }
}
