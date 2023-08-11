using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Setame.Data.Handlers.EnvironmentSets;

namespace Setame.Data.Handlers.Users;

[ExcludeFromCodeCoverage]
public class RequestPasswordResetValidator : AbstractValidator<RequestPasswordReset>
{
    public RequestPasswordResetValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty().EmailAddress().WithMessage("a valid {PropertyName} is required.").WithErrorCode("Vx1008");
    }
}

