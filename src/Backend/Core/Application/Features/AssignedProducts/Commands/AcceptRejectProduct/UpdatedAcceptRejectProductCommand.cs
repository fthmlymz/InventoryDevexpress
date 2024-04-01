using Application.Features.AssignedProducts.Dtos;
using MediatR;
using Shared;

namespace Application.Features.AssignedProducts.Commands.AcceptRejectProduct
{
    public sealed record UpdatedAcceptRejectProductCommand(int? ProductId, int? AssignedProductId, string? ApprovalStatus
                                                          ) : IRequest<Result<AssignedProductDto>>;

}
