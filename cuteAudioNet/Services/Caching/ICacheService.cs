namespace cuteAudioNet.Services.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task SetAsync<T>(string key, T value, TimeSpan Time);
        Task<long> IncrementAsync(string key);
        Task<long> GetVersionAsync(string key);
    }
}