using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.CategoriesSub.Commands.CreateCategorySub
{
    public sealed record CreateCategorySubCommand(string Name, int CategoryId, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedCategorySubDto>>;
}
