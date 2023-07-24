using FluentValidation;

namespace ConfigMan.Data.Handlers.Applications;

public class RenameApplicationValidator : AbstractValidator<RenameApplication>
{
    public RenameApplicationValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1000");

        RuleFor(x => x.NewName)
            .NotEmpty().WithMessage("Name is required.").WithErrorCode("Vx1001")
            .Must(ValidationHelper.BeValidString).WithMessage("{PropertyName} contains non-printable characters.").WithErrorCode("Vx1002")
            .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.").WithErrorCode("Vx1003");
    }


}