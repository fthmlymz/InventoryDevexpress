using MediatR;
using Shared;

namespace Application.Features.Brands.Commands.CreateBrand
{
    public sealed record CreateBrandCommand(string Name, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedBrandDto>>;
}
