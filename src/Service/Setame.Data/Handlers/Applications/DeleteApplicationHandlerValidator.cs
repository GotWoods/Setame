using FluentValidation;

namespace Setame.Data.Handlers.Applications;

public class DeleteApplicationHandlerValidator : AbstractValidator<DeleteApplication>
{
    public DeleteApplicationHandlerValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("{PropertyName} is required.").WithErrorCode("Vx1007");
    }
}