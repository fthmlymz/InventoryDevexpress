using Domain.Common;
using Domain.Entities;

namespace Application.Features.CategoriesSub.Commands.UpdateCategorySub
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
