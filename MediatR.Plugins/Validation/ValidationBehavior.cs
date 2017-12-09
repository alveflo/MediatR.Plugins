using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Plugins.Validation
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<IValidatableRequest<TResponse>, TResponse>
    {
        public async Task<TResponse> Handle(IValidatableRequest<TResponse> request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var validationResult = request.Validate();
            var failures = validationResult.Errors.Where(x => x != null).ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            var response = await next();
            return response;
        }
    }
}
