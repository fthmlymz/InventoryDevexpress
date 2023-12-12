using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Categories.Commands.CreateCategory
{
    public class CategoryCreatedEvent : BaseEvent
    {
        public Domain.Entities.Category Category { get; }

        public CategoryCreatedEvent(Domain.Entities.Category category)
        {
            Category = category;
        }
    }
}
