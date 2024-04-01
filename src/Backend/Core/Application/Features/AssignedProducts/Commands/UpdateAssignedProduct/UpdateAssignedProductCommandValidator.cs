using FluentValidation;

namespace Application.Features.AssignedProducts.Commands.UpdateAssignedProduct
{
    public class UpdateAssignedProductCommandValidator : AbstractValidator<UpdateAssignedProductCommand>
    {
        public UpdateAssignedProductCommandValidator()
        {
            RuleFor(x => x.ProductId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
