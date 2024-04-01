namespace Application.Interfaces.Services
{
    public interface IEasyCacheService
    {
        Task<object> GetAsync(string key, Type cachedData);
        Task<T> GetAsync<T>(string key);
        Task<bool> GetAnyAsync(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefix);
    }
}
