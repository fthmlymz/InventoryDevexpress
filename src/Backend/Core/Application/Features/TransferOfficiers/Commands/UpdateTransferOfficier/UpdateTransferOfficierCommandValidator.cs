using FluentValidation;

namespace Application.Features.TransferOfficiers.Commands.UpdateTransferOfficier
{
    public class UpdateTransferOfficierCommandValidator : AbstractValidator<UpdateTransferOfficierCommand>
    {
        public UpdateTransferOfficierCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.CompanyId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
