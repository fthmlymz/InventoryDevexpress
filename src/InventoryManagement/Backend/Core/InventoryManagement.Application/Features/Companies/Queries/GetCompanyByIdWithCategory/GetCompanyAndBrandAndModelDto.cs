using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Companies.Queries.GetCompanyByIdWithCategory
{
    public class GetCompanyAndBrandAndModelDto : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public int? BrandId { get; set; }
    }
}
