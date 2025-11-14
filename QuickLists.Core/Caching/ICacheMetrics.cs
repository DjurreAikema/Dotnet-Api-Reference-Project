namespace QuickLists.Core.Caching;

public interface ICacheMetrics
{
    void RecordHit();
    void RecordMiss();
    CacheStatistics GetStatistics();
}

public record CacheStatistics(
    long TotalHits,
    long TotalMisses,
    double HitRate
);