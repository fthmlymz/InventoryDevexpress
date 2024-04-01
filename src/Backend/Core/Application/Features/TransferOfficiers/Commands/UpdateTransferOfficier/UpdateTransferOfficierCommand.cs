using Domain.Entities;
using MediatR;
using Shared;

namespace Application.Features.TransferOfficiers.Commands.UpdateTransferOfficier
{
    public sealed record class UpdateTransferOfficierCommand(int Id, string? Email, string? FullName, string? UserName, int? CompanyId, string? UpdatedBy,
                                                             string UpdatedUserId) : IRequest<Result<TransferOfficier>>;
}
