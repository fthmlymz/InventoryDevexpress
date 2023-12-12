using InventoryManagement.Application.Features.Companies.Queries.GetCompanyByIdWithCategory;
using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryListWithPaginationQuery
{
    public class GetCategoryWithPaginationDto : BaseAuditableEntity
    {
        public string? Name { get; set; }




        #region Relationship - Affiliated with the upper class
        public int CompanyId { get; set; }
        public List<GetCompanyAndCategoryAndCategorySubDto> CategorySubs { get; set; } = new();
        #endregion
    }
}
