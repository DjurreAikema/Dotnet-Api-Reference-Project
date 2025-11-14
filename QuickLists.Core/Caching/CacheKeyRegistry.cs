using System.Collections.Concurrent;

namespace QuickLists.Core.Caching;

public class CacheKeyRegistry : ICacheKeyRegistry
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _keysByPattern = new();

    public void Register(string cacheKey, string pattern)
    {
        _keysByPattern.AddOrUpdate(
            pattern,
            _ => [cacheKey],
            (_, existing) =>
            {
                existing.Add(cacheKey);
                return existing;
            });
    }

    public IEnumerable<string> GetKeysMatchingPattern(string pattern)
    {
        return _keysByPattern.TryGetValue(pattern, out var keys)
            ? keys.ToList()
            : [];
    }
}