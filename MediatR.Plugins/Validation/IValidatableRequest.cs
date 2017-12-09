using FluentValidation.Results;

namespace MediatR.Plugins.Validation
{
    public interface IValidatableRequest<TResponse> : IRequest<TResponse>
    {
        ValidationResult Validate();
    }
}
