using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.CategoriesSub.Commands.CreateCategorySub
{
    public class CreateCategorySubEvent : BaseEvent, INotification
    {
        public CategorySub CategorySub { get; }

        public CreateCategorySubEvent(CategorySub categorySub)
        {
            CategorySub = categorySub;
        }
    }
}
