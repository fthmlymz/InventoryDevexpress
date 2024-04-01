using Domain.Common;
using Domain.Entities;

namespace Application.Features.CategoriesSub.Commands.DeleteCategorySub
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
