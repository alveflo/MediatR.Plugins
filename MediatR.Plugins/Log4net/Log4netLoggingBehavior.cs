using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.Threading.Tasks;
using System.Threading;

namespace MediatR.Plugins.Log4net
{
    public class Log4netLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILog _logger;

        public Log4netLoggingBehavior(ILog logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var traceId = Guid.NewGuid();
                _logger.Debug($"Handling {typeof(TRequest).FullName}. TraceId: {traceId} ({DateTime.UtcNow})");
                var response = await next();
                _logger.Debug($"Handled {typeof(TResponse).FullName}. TraceId: {traceId} ({DateTime.UtcNow})");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred while handling {typeof(TRequest).FullName}", ex);
                throw;
            }
        }
    }
}
