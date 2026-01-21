using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using MSABackendTemplate.Persistence.Services;
using Xunit;

namespace MSABackendTemplate.Tests.Persistence.Services;

/// <summary>
/// Unit tests for MemoryCacheService.
/// Tests cache operations (Get, Set, Remove, Pattern matching).
/// </summary>
public class MemoryCacheServiceTests : IDisposable
{
    private readonly MemoryCache _memoryCache;
    private readonly MemoryCacheService _sut;

    public MemoryCacheServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _sut = new MemoryCacheService(_memoryCache);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        // Act
        var result = await _sut.GetAsync<string>("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_AndGetAsync_ShouldStoreAndRetrieveValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";

        // Act
        await _sut.SetAsync(key, value);
        var result = await _sut.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task SetAsync_ShouldExpireAfterGivenTime()
    {
        // Arrange
        var key = "expiring-key";
        var value = "expiring-value";
        var expiration = TimeSpan.FromMilliseconds(100);

        // Act
        await _sut.SetAsync(key, value, expiration);

        // Immediate get should work
        var immediate = await _sut.GetAsync<string>(key);
        immediate.Should().Be(value);

        // Wait for expiration
        await Task.Delay(150);

        var expired = await _sut.GetAsync<string>(key);

        // Assert
        expired.Should().BeNull();
    }

    [Fact]
    public async Task GetOrSetAsync_ShouldReturnCachedValue_WhenExists()
    {
        // Arrange
        var key = "cached-key";
        var cachedValue = "cached-value";
        await _sut.SetAsync(key, cachedValue);

        var factoryCalled = false;

        // Act
        var result = await _sut.GetOrSetAsync(key, async () =>
        {
            factoryCalled = true;
            return "new-value";
        });

        // Assert
        result.Should().Be(cachedValue);
        factoryCalled.Should().BeFalse(); // Factory should NOT be called
    }

    [Fact]
    public async Task GetOrSetAsync_ShouldCallFactoryAndCache_WhenNotExists()
    {
        // Arrange
        var key = "new-key";
        var factoryValue = "factory-value";

        // Act
        var result = await _sut.GetOrSetAsync(key, async () =>
        {
            return factoryValue;
        });

        // Assert
        result.Should().Be(factoryValue);

        // Verify it was cached
        var cached = await _sut.GetAsync<string>(key);
        cached.Should().Be(factoryValue);
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveValue()
    {
        // Arrange
        var key = "remove-key";
        var value = "remove-value";
        await _sut.SetAsync(key, value);

        // Act
        await _sut.RemoveAsync(key);
        var result = await _sut.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveByPatternAsync_ShouldRemoveMatchingKeys()
    {
        // Arrange
        await _sut.SetAsync("products:all", "all products");
        await _sut.SetAsync("products:page:1", "page 1");
        await _sut.SetAsync("products:page:2", "page 2");
        await _sut.SetAsync("users:all", "all users");

        // Act
        await _sut.RemoveByPatternAsync("products:*");

        // Assert
        var products1 = await _sut.GetAsync<string>("products:all");
        var products2 = await _sut.GetAsync<string>("products:page:1");
        var products3 = await _sut.GetAsync<string>("products:page:2");
        var users = await _sut.GetAsync<string>("users:all");

        products1.Should().BeNull();
        products2.Should().BeNull();
        products3.Should().BeNull();
        users.Should().NotBeNull(); // Should NOT be removed
    }

    [Fact]
    public async Task RemoveByPatternAsync_ShouldHandleComplexPatterns()
    {
        // Arrange
        await _sut.SetAsync("product:123:details", "details");
        await _sut.SetAsync("product:456:details", "details");
        await _sut.SetAsync("product:123:reviews", "reviews");

        // Act
        await _sut.RemoveByPatternAsync("product:123:*");

        // Assert
        var details123 = await _sut.GetAsync<string>("product:123:details");
        var details456 = await _sut.GetAsync<string>("product:456:details");
        var reviews123 = await _sut.GetAsync<string>("product:123:reviews");

        details123.Should().BeNull();
        reviews123.Should().BeNull();
        details456.Should().NotBeNull(); // Different product
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
    }
}
