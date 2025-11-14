namespace QuickLists.Core.Caching;

public class CacheMetrics : ICacheMetrics
{
    private long _hits;
    private long _misses;

    public void RecordHit()
    {
        Interlocked.Increment(ref _hits);
    }

    public void RecordMiss()
    {
        Interlocked.Increment(ref _misses);
    }

    public CacheStatistics GetStatistics()
    {
        var hits = Interlocked.Read(ref _hits);
        var misses = Interlocked.Read(ref _misses);
        var total = hits + misses;
        var hitRate = total == 0 ? 0 : (double) hits / total;

        return new CacheStatistics(hits, misses, hitRate);
    }
}