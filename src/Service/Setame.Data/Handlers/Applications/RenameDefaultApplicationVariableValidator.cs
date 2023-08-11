using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Setame.Data.Handlers.Applications;

[ExcludeFromCodeCoverage]
public class RenameDefaultApplicationVariableValidator : AbstractValidator<RenameDefaultApplicationVariable>
{
    public RenameDefaultApplicationVariableValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1000");

        RuleFor(x => x.NewName)
            .NotEmpty().WithMessage("Name is required.").WithErrorCode("Vx1001")
            .Must(ValidationHelper.BeValidString).WithMessage("{PropertyName} contains non-printable characters.").WithErrorCode("Vx1002")
            .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.").WithErrorCode("Vx1003");
    }
}