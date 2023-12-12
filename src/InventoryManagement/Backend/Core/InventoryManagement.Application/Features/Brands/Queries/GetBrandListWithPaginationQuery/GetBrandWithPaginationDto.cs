using InventoryManagement.Application.Features.Companies.Queries.GetCompanyByIdWithCategory;
using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Brands.Queries.GetBrandListWithPaginationQuery
{
    public class GetBrandWithPaginationDto : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;




        #region Relationship - Affiliated with the upper class
        public int CompanyId { get; set; }
        public List<GetCompanyAndBrandAndModelDto> Models { get; set; } = new();
        #endregion
    }
}
