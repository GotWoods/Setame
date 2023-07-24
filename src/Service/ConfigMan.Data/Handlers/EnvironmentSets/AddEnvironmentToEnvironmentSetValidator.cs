using ConfigMan.Data.Handlers.Applications;
using FluentValidation;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public class AddEnvironmentToEnvironmentSetValidator : AbstractValidator<AddEnvironmentToEnvironmentSet>
{
    public AddEnvironmentToEnvironmentSetValidator()
    {
        RuleFor(x => x.EnvironmentSetId)
            .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1000");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.").WithErrorCode("Vx1001")
            .Must(ValidationHelper.BeValidString).WithMessage("{PropertyName} contains non-printable characters.").WithErrorCode("Vx1002")
            .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.").WithErrorCode("Vx1003");
    }
}

