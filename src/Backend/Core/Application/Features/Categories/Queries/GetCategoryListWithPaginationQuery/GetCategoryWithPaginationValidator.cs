﻿using FluentValidation;

namespace Application.Features.Categories.Queries.GetCategoryListWithPaginationQuery
{
    public class GetCategoryWithPaginationValidator : AbstractValidator<GetCategoryWithPaginationQuery>
    {
        public GetCategoryWithPaginationValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).WithMessage("PageSize least greater than or equal to 1.");
        }
    }
}
