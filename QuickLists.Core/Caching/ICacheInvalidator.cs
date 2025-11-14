namespace QuickLists.Core.Caching;

public interface ICacheInvalidator
{
    IEnumerable<string> CacheKeysToInvalidate { get; }
}