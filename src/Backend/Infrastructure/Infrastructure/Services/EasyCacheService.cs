using EasyCaching.Core;
using Microsoft.Extensions.Configuration;
using Application.Interfaces.Services;

namespace Infrastructure.Services
{
    public class EasyCacheService : IEasyCacheService
    {
        private readonly IEasyCachingProvider _easyCachingProvider;
        private readonly IConfiguration _configuration;

        public EasyCacheService(IEasyCachingProvider easyCachingProvider, IConfiguration configuration)
        {
            _easyCachingProvider = easyCachingProvider;
            _configuration = configuration;
        }
        public async Task<object> GetAsync(string key, Type cachedData)
        {
            return await _easyCachingProvider.GetAsync(key, cachedData);
        }
        public async Task<T> GetAsync<T>(string key)
        {
            var cachedData = await _easyCachingProvider.GetAsync<T>(key);
            return cachedData.Value;
        }
        public async Task RemoveAsync(string key)
        {
            await _easyCachingProvider.RemoveAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            await _easyCachingProvider.RemoveByPrefixAsync(prefix);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            //if (expiration.HasValue)
            //{
            //    await _easyCachingProvider.SetAsync(key, value, expiration.Value);
            //}
            //else
            //{
            //    await _easyCachingProvider.SetAsync(key, value, TimeSpan.FromMinutes(10));
            //}
            if (expiration.HasValue)
            {
                await _easyCachingProvider.SetAsync(key, value, expiration.Value);
            }
            else
            {
                var defaultExpiration = TimeSpan.FromMinutes(5);
                var appSettingsExpiration = _configuration.GetValue("RedisConnectionSettings:DefaultExpiration", defaultExpiration);
                await _easyCachingProvider.SetAsync(key, value, appSettingsExpiration);
            }
        }

        public async Task<bool> GetAnyAsync(string key)
        {
            var cachedValue = await _easyCachingProvider.GetAsync<bool?>(key);
            return cachedValue?.Value ?? false;
        }
    }

}


/*
 public class EasyCacheService : IEasyCacheService
    {
        private readonly IEasyCachingProvider _easyCachingProvider;

        public EasyCacheService(IEasyCachingProvider easyCachingProvider)
        {
            _easyCachingProvider = easyCachingProvider;
        }

     
        public async Task<object> GetAsync(string key, Type cachedData)
        {
            return await _easyCachingProvider.GetAsync(key, cachedData);
        }
        public async Task<bool> GetAnyAsync(string key)
        {
            var cachedValue = await _easyCachingProvider.GetAsync<CacheValue<bool?>>(key);
            return cachedValue?.Value ?? false;
        }
        public async Task RemoveAsync(string key)
        {
            await _easyCachingProvider.RemoveAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            await _easyCachingProvider.RemoveByPrefixAsync(prefix);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (expiration.HasValue)
            {
                await _easyCachingProvider.SetAsync(key, value, expiration.Value);
            }
            else
            {
                await _easyCachingProvider.SetAsync(key, value, TimeSpan.FromMinutes(10));
            }
        }
    }
 */