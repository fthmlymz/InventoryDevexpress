using FluentValidation;

namespace InventoryManagement.Application.Features.CategoriesSub.Commands.DeleteCategorySub
{
    public class DeleteCategorySubCommandValidator : AbstractValidator<DeleteCategorySubCommand>
    {
        public DeleteCategorySubCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
