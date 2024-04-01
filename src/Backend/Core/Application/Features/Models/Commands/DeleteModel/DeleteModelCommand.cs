using MediatR;

namespace Application.Features.Models.Commands.DeleteModel
{
    public sealed record DeleteModelCommand(int Id) : IRequest<bool>;
}
