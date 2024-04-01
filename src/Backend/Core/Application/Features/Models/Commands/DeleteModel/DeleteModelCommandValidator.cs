using FluentValidation;

namespace Application.Features.Models.Commands.DeleteModel
{
    public class DeleteModelCommandValidator : AbstractValidator<DeleteModelCommand>
    {
        public DeleteModelCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
