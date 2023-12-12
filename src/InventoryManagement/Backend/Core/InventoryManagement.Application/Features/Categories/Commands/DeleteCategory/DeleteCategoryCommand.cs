using MediatR;

namespace InventoryManagement.Application.Features.Categories.Commands.DeleteCategory
{
    public sealed record DeleteCategoryCommand(int Id) : IRequest<bool>;
}
