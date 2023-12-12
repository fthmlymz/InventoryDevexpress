using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.TransferOfficiers.Commands.UpdateTransferOfficier
{
    public sealed record class UpdateTransferOfficierCommand(int Id, string? Name, string? Email, int? CompanyId, string? CreatedBy, 
                                                             string? CreatedUserId, string? UpdatedBy, string UpdatedUserId
                                                            ) : IRequest<Result<TransferOfficier>>;
}
