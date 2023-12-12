using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Companies.Queries.GetCompanyByIdWithCategory
{
    public class GetCompanyAndCategoryAndCategorySubDto : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
    }
}
