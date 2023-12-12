using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Brands.Queries.GetBrandWithModel
{
    public class GetBrandWithModelDto : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? BrandId { get; set; }


        public List<GetBrandWithModelDto> BrandModels { get; set; } = new List<GetBrandWithModelDto>();
    }
}
