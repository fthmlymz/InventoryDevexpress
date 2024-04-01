using FluentValidation;

namespace Application.Features.CategoriesSub.Commands.CreateCategorySub
{
    public class CreateCategorySubCommandValidator : AbstractValidator<CreateCategorySubCommand>
    {
        public CreateCategorySubCommandValidator()
        {
            RuleFor(x => x.Name).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.CategoryId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.CreatedBy).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
            RuleFor(x => x.CreatedUserId).NotNull().WithMessage("{PropertyName} bu alan gereklidir").NotEmpty().WithMessage("{PropertyName} bu alan gereklidir");
        }
    }
}
