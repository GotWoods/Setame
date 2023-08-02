using FluentValidation;
using Setame.Data.Handlers.EnvironmentSets;

namespace Setame.Data.Handlers.Users;

public class ResetPasswordFromTokenValidator : AbstractValidator<ResetPasswordFromToken>
{
    public ResetPasswordFromTokenValidator()
    {

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1001")
            .Must(ValidationHelper.BeValidString).WithMessage("{PropertyName} contains non-printable characters.").WithErrorCode("Vx1002")
            .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.").WithErrorCode("Vx1003");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1001")
            .Must(ValidationHelper.BeValidString).WithMessage("{PropertyName} contains non-printable characters.").WithErrorCode("Vx1002")
            .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.").WithErrorCode("Vx1003");
    }
}

