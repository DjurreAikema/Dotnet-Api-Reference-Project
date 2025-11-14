namespace QuickLists.Core.Caching;

public interface ICacheKeyRegistry
{
    void Register(string cacheKey, string pattern);
    IEnumerable<string> GetKeysMatchingPattern(string pattern);
}