using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ConfigMan.Data.Handlers.Applications
{
    public class CreateDefaultApplicationVariableValidator : AbstractValidator<CreateDefaultApplicationVariable>
    {
        public CreateDefaultApplicationVariableValidator()
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1007");
            
            RuleFor(x => x.VariableName)
                .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1011")
                .Must(ValidationHelper.BeValidString).WithMessage("{PropertyName} contains non-printable characters.").WithErrorCode("Vx1012")
                .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.").WithErrorCode("Vx1013");

        }
    }
}
