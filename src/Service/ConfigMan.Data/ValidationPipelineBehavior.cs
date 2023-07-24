using ConfigMan.Data.Handlers;
using FluentValidation;
using MediatR;

namespace ConfigMan.Data;

//got this technique from https://www.youtube.com/watch?v=85dxwd8HzEk&t=1s
public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResponse
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        Errors[] errors = _validators.Select(validator => validator.Validate(request))
            .SelectMany(validationResult=>validationResult.Errors)
            .Where(validationFailure=>validationFailure is not null)
            .Select(failure => Errors.FromValidation(failure.ErrorCode, failure.ErrorMessage))
            .Distinct().ToArray();

        if (errors.Any())
        {
            var result = new CommandResponse();
            result.Errors.AddRange(errors);
            return (TResponse)result;
        }

        return await next();
    }
}