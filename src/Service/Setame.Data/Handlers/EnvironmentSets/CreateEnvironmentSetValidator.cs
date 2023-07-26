using FluentValidation;

namespace Setame.Data.Handlers.EnvironmentSets;

public class CreateEnvironmentSetValidator : AbstractValidator<CreateEnvironmentSet>
{
    public CreateEnvironmentSetValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1001")
            .Must(ValidationHelper.BeValidString).WithMessage("{PropertyName} contains non-printable characters.").WithErrorCode("Vx1002")
            .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.").WithErrorCode("Vx1003");
    }
}

