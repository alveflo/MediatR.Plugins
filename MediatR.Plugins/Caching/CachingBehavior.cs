using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Plugins.Caching
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<ICachableRequest<TResponse>, TResponse>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ICacheLogger _logger;

        public CachingBehavior(ICacheProvider cacheProvider, ICacheLogger logger)
        {
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        public async Task<TResponse> Handle(ICachableRequest<TResponse> request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var cacheKey = request.GetCacheKey();
            var cachedResponse = (TResponse) _cacheProvider.Get(cacheKey);
            if (cachedResponse != null)
            {
                _logger.Debug($"Response retrieved {typeof(TRequest).FullName} from cache. Cachekey: {cacheKey}");
                return cachedResponse;
            }

            var response = await next();
            _logger.Debug($"Caching response for {typeof(TRequest).FullName} with cache key: {cacheKey}");

            _cacheProvider.Add(cacheKey, response);
            return response;
        }
    }
}
