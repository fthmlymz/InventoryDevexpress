using FluentValidation;

namespace InventoryManagement.Application.Features.Companies.Queries.GetCompanyWithPagination
{
    public class GetCompanyListWithPaginationValidator : AbstractValidator<GetCompanyListWithPaginationQuery>
    {
        public GetCompanyListWithPaginationValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1).WithMessage("{PropertyName} en az 1'den büyük veya eşit olmalıdır.");
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).WithMessage("{PropertyName} en az 1'e eşit veya daha büyük olmalıdır.");
        }
    }
}
