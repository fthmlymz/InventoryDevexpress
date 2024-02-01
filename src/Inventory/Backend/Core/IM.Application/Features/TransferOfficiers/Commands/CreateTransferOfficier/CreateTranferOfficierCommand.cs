using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.TransferOfficiers.Commands.CreateTransferOfficier
{
    public sealed record CreateTranferOfficierCommand(string? FullName, string? UserName, string? Email, int? CompanyId, string? CreatedBy, string? CreatedUserId
                                                     ) : IRequest<Result<TransferOfficierDto>>;
}
