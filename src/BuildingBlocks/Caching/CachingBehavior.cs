using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Caching
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly ICacheRequest _cacheRequest;
        private readonly int defaultCacheExpirationInHours = 1;

        public CachingBehavior(IEasyCachingProviderFactory cachingFactory,
            ILogger<CachingBehavior<TRequest, TResponse>> logger,
            ICacheRequest cacheRequest)
        {
            _logger = logger;
            _cachingProvider = cachingFactory.GetCachingProvider("mem");
            _cacheRequest = cacheRequest;
        }


        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {

            if (request is not ICacheRequest || _cacheRequest == null)
            {
                // No cache request found, so just continue through the pipeline
                return await next();
            }

            var cacheKey = _cacheRequest.CacheKey;
            var cachedResponse = await _cachingProvider.GetAsync<TResponse>(cacheKey);
            if (cachedResponse.Value != null)
            {
                _logger.LogDebug("Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                    typeof(TRequest).FullName, cacheKey);
                return cachedResponse.Value;
            }

            var response = await next();

            var expirationTime = _cacheRequest.AbsoluteExpirationRelativeToNow ??
                                 DateTime.Now.AddHours(defaultCacheExpirationInHours);

            await _cachingProvider.SetAsync(cacheKey, response, expirationTime.TimeOfDay);

            _logger.LogDebug("Caching response for {TRequest} with cache key: {CacheKey}", typeof(TRequest).FullName,
                cacheKey);

            return response;
        }
    }
}
