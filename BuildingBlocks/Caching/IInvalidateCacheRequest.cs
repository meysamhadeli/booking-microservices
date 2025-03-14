namespace BuildingBlocks.Caching
{
    public interface IInvalidateCacheRequest
    {
        string CacheKey { get; }
    }
}
