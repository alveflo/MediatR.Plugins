using MediatR.Plugins.Caching;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Plugins.Tests.Handlers
{
    public class CachablePing
    {
        public class Handler : IRequestHandler<Request, Result>
        {
            public Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Result
                {
                    Value = Math.Pow(request.Value, 2),
                    ProcessedTicks = DateTime.UtcNow.Ticks
                });
            }
        }

        public class Request : ICachableRequest<Result>
        {
            public double Value { get; set; }

            public string GetCacheKey()
            {
                return Value.ToString();
            }
        }

        public class Result
        {
            public double Value { get; set; }
            public long ProcessedTicks { get; set; }
        }
    }
}
