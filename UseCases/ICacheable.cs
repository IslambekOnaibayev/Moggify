namespace UseCases
{
    public interface ICacheable
    {
        string GetCacheKey();
        string? CacheProfile { get; }
    }
}
