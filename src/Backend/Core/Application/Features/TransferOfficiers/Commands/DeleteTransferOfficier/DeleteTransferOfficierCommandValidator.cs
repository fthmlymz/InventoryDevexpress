using FluentValidation;

namespace Application.Features.TransferOfficiers.Commands.DeleteTransferOfficier
{
    public sealed class DeleteTransferOfficierCommandValidator : AbstractValidator<DeleteTransferOfficierCommand>
    {
        public DeleteTransferOfficierCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
