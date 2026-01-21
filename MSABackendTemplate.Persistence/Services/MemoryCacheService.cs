using Microsoft.Extensions.Caching.Memory;
using MSABackendTemplate.Application.Interfaces;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace MSABackendTemplate.Persistence.Services;

/// <summary>
/// In-Memory cache implementation using IMemoryCache.
/// For production with multiple servers, consider Redis (distributed cache).
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, bool> _cacheKeys = new();
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
        };
        options.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
        {
            if (evictedKey is string cacheKey)
            {
                _cacheKeys.TryRemove(cacheKey, out _);
            }
        });

        _cache.Set(key, value, options);
        _cacheKeys.TryAdd(key, true);

        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
    {
        // Try to get from cache
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
            return cachedValue;

        // Not in cache, get from factory
        var value = await factory();

        // Store in cache
        await SetAsync(key, value, expiration);

        return value;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        _cacheKeys.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        // Convert glob pattern to regex (e.g., "products:*" -> "^products:.*$")
        var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        var regex = new Regex(regexPattern);

        var keysToRemove = _cacheKeys.Keys.Where(k => regex.IsMatch(k)).ToList();

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _cacheKeys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }
}
