using FluentValidation;

namespace InventoryManagement.Application.Features.Brands.Queries.GetBrandListWithPaginationQuery
{
    public class GetBrandWithPaginationValidator : AbstractValidator<GetBrandWithPaginationQuery>
    {
        public GetBrandWithPaginationValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).WithMessage("PageSize least greater than or equal to 1.");
        }
    }
}
