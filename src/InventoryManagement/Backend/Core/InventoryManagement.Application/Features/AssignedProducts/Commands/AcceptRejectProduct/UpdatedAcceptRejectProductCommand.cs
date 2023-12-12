using InventoryManagement.Application.Features.AssignedProducts.Dtos;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.AssignedProducts.Commands.AcceptRejectProduct
{
    public sealed record UpdatedAcceptRejectProductCommand( int? ProductId, int? AssignedProductId, string? ApprovalStatus
                                                          ) : IRequest<Result<AssignedProductDto>>;
    
}
