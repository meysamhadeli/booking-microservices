using System;
using System.Linq;
using MediatR;

namespace BuildingBlocks.Caching
{
    public interface IInvalidateCacheRequest
    {
        string CacheKey { get; }
    }
}
