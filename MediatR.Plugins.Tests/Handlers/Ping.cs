using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Plugins.Tests.Handlers
{
    public class Ping
    {
        public class Handler : IRequestHandler<Request, Result>
        {
            public Handler()
            {
            }

            public Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                if (request.Value > 100) throw new Exception();

                return Task.FromResult(new Result { Value = request.Value });
            }
        }

        public class Request : IRequest<Result>
        {
            public int Value { get; set; }
        }
        public class Result
        {
            public int Value { get; set; }
        }
    }
}
