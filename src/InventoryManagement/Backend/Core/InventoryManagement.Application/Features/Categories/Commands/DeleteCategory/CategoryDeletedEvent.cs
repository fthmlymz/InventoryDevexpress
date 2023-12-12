using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Categories.Commands.DeleteCategory
{
    public class CategoryDeletedEvent : BaseEvent
    {
        public Domain.Entities.Category Category { get; }
        public CategoryDeletedEvent(Domain.Entities.Category category)
        {
            Category = category;
        }
    }
}
