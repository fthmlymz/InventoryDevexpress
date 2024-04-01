using MediatR;
using Shared;

namespace Application.Features.Models.Commands.CreateModel
{
    public sealed record CreateModelCommand(string Name, int? BrandId, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedModelDto>>;
}
