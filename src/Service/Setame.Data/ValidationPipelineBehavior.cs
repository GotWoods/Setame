using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Handlers;

namespace Setame.Data;

//got this technique from https://www.youtube.com/watch?v=85dxwd8HzEk&t=1s
public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResponse
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationPipelineBehavior<TRequest, TResponse>> _logger;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            _logger.LogDebug("No validators for {Type}", typeof(TRequest).FullName);
            return await next();
        }

        var errors = _validators.Select(validator => validator.Validate(request))
            .SelectMany(validationResult=>validationResult.Errors)
            .Where(validationFailure=>validationFailure is not null)
            .Select(failure => Errors.FromValidation(failure.ErrorCode, failure.ErrorMessage))
            .Distinct().ToArray();

        if (errors.Any())
        {
            _logger.LogWarning("Validator found errors for {Type}: {@Errors}", typeof(TRequest).FullName, errors);
            var result = new CommandResponse();
            result.Errors.AddRange(errors);
            return (TResponse)result;
        }

        return await next();
    }
}