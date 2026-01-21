namespace MSABackendTemplate.Application.Interfaces;

/// <summary>
/// Cache service interface for storing and retrieving cached data.
/// Abstraction allows switching between in-memory and distributed cache (Redis).
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key. Returns null if not found.
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Sets a value in cache with specified expiration time.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Gets cached value or sets it if not exists (Cache-Aside pattern).
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Removes a value from cache by key.
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes all cached values matching a pattern (e.g., "products:*").
    /// </summary>
    Task RemoveByPatternAsync(string pattern);
}
