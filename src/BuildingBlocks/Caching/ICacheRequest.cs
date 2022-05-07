using MediatR;

namespace BuildingBlocks.Caching;

public interface ICacheRequest
{
    string CacheKey { get; }
    DateTime? AbsoluteExpirationRelativeToNow { get; }
}
