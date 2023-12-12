using MediatR;

namespace InventoryManagement.Application.Features.Models.Commands.DeleteModel
{
    public sealed record DeleteModelCommand(int Id) : IRequest<bool>;
}
