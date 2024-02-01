using InventoryManagement.Application.Features.Products.GeneralDtos;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Products.Commands.FileTransfer
{
    public sealed record FileTransferCommand(string Description, int ProductId, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedProductMovementDto>>;
}
