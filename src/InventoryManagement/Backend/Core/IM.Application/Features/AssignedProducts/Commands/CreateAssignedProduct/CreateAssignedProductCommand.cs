using InventoryManagement.Application.Features.AssignedProducts.Dtos;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.AssignedProducts.Commands.CreateAssignedProduct
{
    public sealed record CreateAssignedProductCommand(string? AssignedUserName, string? AssignedUserId, int? ProductId, string? AssignedUserPhoto,
                                                      string? FullName, string? Email, string? Barcode, string? ProductName,
                                                      string? CreatedBy, string? CreatedUserId) : IRequest<Result<AssignedProductDto>>;
}
