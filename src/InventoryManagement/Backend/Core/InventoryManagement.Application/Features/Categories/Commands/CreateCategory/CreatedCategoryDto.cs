using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Categories.Commands.CreateCategory
{
    public class CreatedCategoryDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
    }
}
