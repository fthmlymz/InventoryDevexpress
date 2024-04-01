using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Categories.Commands.CreateCategory
{
    public sealed record CreateCategoryCommand(string Name, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedCategoryDto>>;
}
