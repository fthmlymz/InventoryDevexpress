using FluentValidation;

namespace InventoryManagement.Application.Features.Products.Queries.GetProductListWithPaginationQuery
{
    public class GetProductWithPaginationValidator : AbstractValidator<GetProductWithPaginationQuery>
    {
        public GetProductWithPaginationValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).WithMessage("PageSize least greater than or equal to 1.");
        }
    }
}
