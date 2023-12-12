using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryWithCategorySub
{
    public class GetCategoryWithCategorySubDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
        public int? CompanyId { get; set; }
        public int? CategoryId { get; set; }

        public List<GetCategoryWithCategorySubDto> CategorySubs { get; set; } = new List<GetCategoryWithCategorySubDto>();
    }
}
