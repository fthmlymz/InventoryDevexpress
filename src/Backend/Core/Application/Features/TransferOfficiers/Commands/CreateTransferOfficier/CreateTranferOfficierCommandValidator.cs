using FluentValidation;

namespace Application.Features.TransferOfficiers.Commands.CreateTransferOfficier
{
    public class CreateTranferOfficierCommandValidator : AbstractValidator<CreateTranferOfficierCommand>
    {
        public CreateTranferOfficierCommandValidator()
        {
            RuleFor(x => x.CompanyId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.Email).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
