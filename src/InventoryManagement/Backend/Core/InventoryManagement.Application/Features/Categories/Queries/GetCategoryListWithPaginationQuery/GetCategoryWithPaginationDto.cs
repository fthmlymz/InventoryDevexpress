using InventoryManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryListWithPaginationQuery
{
    public class GetCategoryWithPaginationDto : BaseAuditableEntity
    {
        public string? Name { get; set; }

        public List<CategorySubModel>? CategorySubs { get; set; } = new List<CategorySubModel>();
    }
    public class CategorySubModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Alt kategori adı gereklidir")]
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
