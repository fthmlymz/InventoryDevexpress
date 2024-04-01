using MediatR;

namespace Application.Features.TransferOfficiers.Commands.DeleteTransferOfficier
{
    public sealed record DeleteTransferOfficierCommand(int Id) : IRequest<bool>;
}
