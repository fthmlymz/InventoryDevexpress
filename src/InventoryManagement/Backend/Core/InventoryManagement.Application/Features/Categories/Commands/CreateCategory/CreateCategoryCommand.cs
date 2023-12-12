using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Categories.Commands.CreateCategory
{
    public sealed record CreateCategoryCommand(string Name, int? CompanyId, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedCategoryDto>>;
}
