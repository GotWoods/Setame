using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ConfigMan.Data.Handlers.Applications
{
    public class CreateApplicationVariableValidator : AbstractValidator<CreateApplicationVariable>
    {
        public CreateApplicationVariableValidator()
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithMessage("ApplicationId is required.").WithErrorCode("Vx1007");

            RuleFor(x => x.Environment)
                .NotEmpty().WithMessage("Environment is required.").WithErrorCode("Vx1008")
                .Must(ValidationHelper.BeValidString).WithMessage("Environment contains non-printable characters.").WithErrorCode("Vx1009")
                .MaximumLength(100).WithMessage("Environment name cannot exceed 100 characters.").WithErrorCode("Vx1010");

            RuleFor(x => x.VariableName)
                .NotEmpty().WithMessage("VariableName is required.").WithErrorCode("Vx1011")
                .Must(ValidationHelper.BeValidString).WithMessage("VariableName contains non-printable characters.").WithErrorCode("Vx1012")
                .MaximumLength(100).WithMessage("VariableName cannot exceed 100 characters.").WithErrorCode("Vx1013");

        }
    }
}
