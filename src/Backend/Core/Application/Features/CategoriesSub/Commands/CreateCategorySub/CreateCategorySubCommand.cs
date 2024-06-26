﻿using MediatR;
using Shared;

namespace Application.Features.CategoriesSub.Commands.CreateCategorySub
{
    public sealed record CreateCategorySubCommand(string Name, int CategoryId, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedCategorySubDto>>;
}
