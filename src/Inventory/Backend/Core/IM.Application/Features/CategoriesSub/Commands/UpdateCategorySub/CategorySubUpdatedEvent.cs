using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.CategoriesSub.Commands.UpdateCategorySub
{
    public class CategorySubUpdatedEvent : BaseEvent
    {
        public CategorySub CategorySub { get; }
        public CategorySubUpdatedEvent(CategorySub categorySub)
        {
            CategorySub = categorySub;
        }
    }
}
