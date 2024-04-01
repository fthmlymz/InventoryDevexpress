using FluentValidation;

namespace InventoryManagement.Application.Features.CategoriesSub.Commands.UpdateCategorySub
{
    public class UpdateCategorySubCommandValidator : AbstractValidator<UpdateCategorySubCommand>
    {
        public UpdateCategorySubCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.Name).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.CategoryId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.UpdatedBy).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.UpdatedUserId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
