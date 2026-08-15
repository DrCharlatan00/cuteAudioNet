using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace cuteAudioNet.Services.Caching
{
    public class RedisCacheService(IDistributedCache _cache) : ICacheService
    {
        private readonly IDistributedCache cache = _cache;

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var json = await cache.GetAsync(key);

                if (json is null) return default;

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan Time)
        {
            var json = JsonSerializer.Serialize(value);

            var opt = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = Time,
            };
            try
            {
                await _cache.SetStringAsync(key, json, opt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
