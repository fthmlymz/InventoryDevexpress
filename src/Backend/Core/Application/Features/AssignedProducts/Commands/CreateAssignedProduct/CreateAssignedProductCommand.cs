using Application.Features.AssignedProducts.Dtos;
using MediatR;
using Shared;

namespace Application.Features.AssignedProducts.Commands.CreateAssignedProduct
{
    public sealed record CreateAssignedProductCommand(string? AssignedUserName, string? AssignedUserId, int? ProductId, string? AssignedUserPhoto,
                                                      string? FullName, string? Email, int? Barcode, string? ProductName, string? Company,
                                                      string? PhysicalDeliveryOfficeName, string? Title, string? Manager, string? Department,
                                                      string? CreatedBy, string? CreatedUserId) : IRequest<Result<AssignedProductDto>>;
}
