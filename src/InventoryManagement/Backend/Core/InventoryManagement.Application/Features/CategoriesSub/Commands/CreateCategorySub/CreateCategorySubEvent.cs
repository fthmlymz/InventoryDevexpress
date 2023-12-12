using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;
using MediatR;

namespace InventoryManagement.Application.Features.CategoriesSub.Commands.CreateCategorySub
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
