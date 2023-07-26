using FluentValidation;

namespace Setame.Data.Handlers.Applications
{
    public class CreateApplicationVariableValidator : AbstractValidator<CreateApplicationVariable>
    {
        public CreateApplicationVariableValidator()
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1007");

            RuleFor(x => x.Environment)
                .NotEmpty().WithMessage("Environment is required.").WithErrorCode("Vx1008")
                .Must(ValidationHelper.BeValidString).WithMessage("{PropertyName} contains non-printable characters.").WithErrorCode("Vx1009")
                .MaximumLength(100).WithMessage("{PropertyName} name cannot exceed 100 characters.").WithErrorCode("Vx1010");

            RuleFor(x => x.VariableName)
                .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1011")
                .Must(ValidationHelper.BeValidString).WithMessage("{PropertyName} contains non-printable characters.").WithErrorCode("Vx1012")
                .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.").WithErrorCode("Vx1013");

        }
    }
}
