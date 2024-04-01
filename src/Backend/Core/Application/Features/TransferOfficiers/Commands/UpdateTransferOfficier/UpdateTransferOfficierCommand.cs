using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.TransferOfficiers.Commands.UpdateTransferOfficier
{
    public sealed record class UpdateTransferOfficierCommand(int Id, string? Email, string?FullName, string? UserName, int? CompanyId, string? UpdatedBy, 
                                                             string UpdatedUserId) : IRequest<Result<TransferOfficier>>;
}
