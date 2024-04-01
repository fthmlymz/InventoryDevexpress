using Application.Features.Products.GeneralDtos;
using MediatR;
using Shared;

namespace Application.Features.Products.Commands.FileTransfer
{
    public sealed record FileTransferCommand(string Description, int ProductId, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedProductMovementDto>>;
}
