using InventoryManagement.Application.Features.Brands.Queries.GetBrandListWithPaginationQuery;
using InventoryManagement.Application.Features.Categories.Queries.GetCategoryListWithPaginationQuery;
using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Companies.Queries.GetCompanyById
{
    //Best performance
    public class GetCompanyByIdWithCategoryDto : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<GetCategoryWithPaginationDto> Categories { get; set; } = new();
        public List<GetBrandWithPaginationDto> Brands { get; set; } = new();
    }
}
