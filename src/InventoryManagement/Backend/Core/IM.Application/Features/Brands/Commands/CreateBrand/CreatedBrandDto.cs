using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Brands.Commands.CreateBrand
{
    public class CreatedBrandDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
    }
}
