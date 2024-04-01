using FluentValidation;

namespace InventoryManagement.Application.Features.Products.Commands.ProductOperations
{
    public class UpdateProductOperationsCommandValidator:AbstractValidator<UpdateProductOperationsCommand>
    {
        public UpdateProductOperationsCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            //RuleFor(x => x.RecipientCompanyId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
