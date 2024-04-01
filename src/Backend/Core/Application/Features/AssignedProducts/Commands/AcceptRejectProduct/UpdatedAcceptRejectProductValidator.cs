using FluentValidation;

namespace Application.Features.AssignedProducts.Commands.AcceptRejectProduct
{
    public class UpdatedAcceptRejectProductValidator : AbstractValidator<UpdatedAcceptRejectProductCommand>
    {
        public UpdatedAcceptRejectProductValidator()
        {
            RuleFor(x => x.ProductId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.AssignedProductId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.ApprovalStatus).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
