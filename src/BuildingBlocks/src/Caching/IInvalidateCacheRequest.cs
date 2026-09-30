namespace Woo.Caching
{
    public interface IInvalidateCacheRequest
    {
        string CacheKey { get; }
    }
}