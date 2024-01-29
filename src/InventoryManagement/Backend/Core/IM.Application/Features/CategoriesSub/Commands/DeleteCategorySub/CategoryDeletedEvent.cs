using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.CategoriesSub.Commands.DeleteCategorySub
{
    public class CategoryDeletedEvent : BaseEvent
    {
        public CategorySub CategorySub { get; }
        public CategoryDeletedEvent(CategorySub categorySub)
        {
            CategorySub = categorySub;
        }
    }
}
