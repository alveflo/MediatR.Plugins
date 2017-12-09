using System.Threading;
using System.Threading.Tasks;
using Serilog;
using System;

namespace MediatR.Plugins.Serilog
{
    public class SerilogLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public SerilogLoggingBehavior(ILogger logger)
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
                _logger.Error(ex, $"An error occurred while handling {typeof(TRequest).FullName}");
                throw;
            }
        }
    }
}
