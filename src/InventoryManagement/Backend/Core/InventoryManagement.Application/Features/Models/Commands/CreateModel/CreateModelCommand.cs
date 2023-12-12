using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Models.Commands.CreateModel
{
    public sealed record CreateModelCommand(string Name, int BrandId, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedModelDto>>;
}
