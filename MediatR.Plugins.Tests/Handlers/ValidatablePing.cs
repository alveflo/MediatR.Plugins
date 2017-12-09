using FluentValidation;
using MediatR.Plugins.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace MediatR.Plugins.Tests.Handlers
{
    public class ValidatablePing
    {
        public class Handler : IRequestHandler<Request, Result>
        {
            public Handler()
            {
            }

            public Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Result
                {
                    Value = request.Value,
                    ProcessedTicks = DateTime.UtcNow.Ticks
                });
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Value)
                    .GreaterThan(10)
                    .LessThan(20);
            }
        }

        public class Request : IValidatableRequest<Result>
        {
            public Request(int value)
            {
                Value = value;
            }

            public int Value { get; set; }
            public ValidationResult Validate() => new Validator().Validate(this);
        }

        public class Result
        {
            public int Value { get; set; }
            public long ProcessedTicks { get; set; }
        }
    }
}
