using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QuickLists.Core.Caching;

namespace QuickLists.Core.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ICacheKeyRegistry _cacheKeyRegistry;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        IMemoryCache cache,
        ICacheKeyRegistry cacheKeyRegistry,
        ILogger<CachingBehavior<TRequest, TResponse>> logger
    )
    {
        _cache = cache;
        _cacheKeyRegistry = cacheKeyRegistry;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is ICacheableQuery cacheableQuery)
        {
            return await HandleCacheableQuery(cacheableQuery, next);
        }

        if (request is ICacheInvalidator cacheInvalidator)
        {
            return await HandleCacheInvalidation(cacheInvalidator, next);
        }

        return await next(cancellationToken);
    }

    private async Task<TResponse> HandleCacheableQuery(
        ICacheableQuery cacheableQuery,
        RequestHandlerDelegate<TResponse> next
    )
    {
        var cacheKey = cacheableQuery.CacheKey;

        // Try to get from the cache
        if (_cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
        {
            _logger.LogInformation(
                "Cache hit for {RequestName} with key {CacheKey}",
                typeof(TRequest).Name,
                cacheKey
            );

            return cachedResponse!;
        }

        // Cache miss
        _logger.LogInformation(
            "Cache miss for {RequestName} with key {CacheKey}",
            typeof(TRequest).Name,
            cacheKey
        );

        var response = await next();

        // Cache the results
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheableQuery.CacheDuration
        };

        _cache.Set(cacheKey, response, cacheOptions);

        // Register the key for pattern-based invalidation
        var pattern = ExtractPattern(cacheKey);
        _cacheKeyRegistry.Register(cacheKey, pattern);

        _logger.LogInformation(
            "Cached response for {RequestName} with key {CacheKey} for {Duration}",
            typeof(TRequest).Name,
            cacheKey,
            cacheableQuery.CacheDuration
        );

        return response;
    }

    private async Task<TResponse> HandleCacheInvalidation(
        ICacheInvalidator cacheInvalidator,
        RequestHandlerDelegate<TResponse> next
    )
    {
        var response = await next();

        foreach (var cacheKeyPattern in cacheInvalidator.CacheKeysToInvalidate)
        {
            // Check if its a wildcard
            if (cacheKeyPattern.EndsWith("*"))
            {
                // Note: IMemoryCache doesn't support pattern removal out of the box
                InvalidateCachePattern(cacheKeyPattern);
            }
            else
            {
                _cache.Remove(cacheKeyPattern);

                _logger.LogInformation(
                    "Invalidated cache key {CacheKey} after {CommandName}",
                    cacheKeyPattern,
                    typeof(TRequest).Name
                );
            }
        }

        return response;
    }

    private void InvalidateCachePattern(string pattern)
    {
        var keysToRemove = _cacheKeyRegistry.GetKeysMatchingPattern(pattern.TrimEnd('*'));


        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);

            _logger.LogInformation(
                "Invalidated cache key {CacheKey} matching pattern {Pattern}",
                key,
                pattern
            );
        }

        if (!keysToRemove.Any())
        {
            _logger.LogDebug(
                "No cache keys found matching pattern {Pattern}",
                pattern
            );
        }
    }

    private static string ExtractPattern(string cacheKey)
    {
        var firstColonIndex = cacheKey.IndexOf(':');
        if (firstColonIndex > 0)
        {
            return cacheKey.Substring(0, firstColonIndex + 1);
        }

        return cacheKey;
    }
}