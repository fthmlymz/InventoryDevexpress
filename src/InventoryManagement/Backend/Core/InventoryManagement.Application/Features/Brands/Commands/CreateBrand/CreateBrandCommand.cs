using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Brands.Commands.CreateBrand
{
    public sealed record CreateBrandCommand(string Name, int? CompanyId, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedBrandDto>>;
}
