namespace QuickLists.Core.Caching;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? CacheDuration => TimeSpan.FromMinutes(5);
}