using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace cuteAudioNet.Services.Caching
{
    public class RedisCacheService(
        IDistributedCache _cache,
        IConnectionMultiplexer redis
        
        ) : ICacheService
    {
        private readonly IDistributedCache cache = _cache;
        private readonly IConnectionMultiplexer redis = redis;

        public async Task<T?> GetAsync<T>(string key)
        {

                var json = await cache.GetAsync(key);

                if (json is null) return default;

                return JsonSerializer.Deserialize<T>(json);
          
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan Time)
        {
            var json = JsonSerializer.Serialize(value);

            var opt = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = Time,
            };
                await _cache.SetStringAsync(key, json, opt);

        }

        public async Task RemoveAsync(string key)
        {
                await _cache.RemoveAsync(key);         
        }

        public async Task<long> IncrementAsync(string key) {
            var database = redis.GetDatabase();
            return  await database.StringIncrementAsync(key);
        }

        public async Task<long> GetVersionAsync(string key) {
            var database = redis.GetDatabase();
            var val = await database.StringGetAsync(key);
            return val.HasValue ? (long)val : 1;
        }
    }
}
