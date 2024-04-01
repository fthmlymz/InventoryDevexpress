using MediatR;
using Shared;

namespace Application.Features.TransferOfficiers.Commands.CreateTransferOfficier
{
    public sealed record CreateTranferOfficierCommand(string? FullName, string? UserName, string? Email, int? CompanyId, string? CreatedBy, string? CreatedUserId
                                                     ) : IRequest<Result<TransferOfficierDto>>;
}
