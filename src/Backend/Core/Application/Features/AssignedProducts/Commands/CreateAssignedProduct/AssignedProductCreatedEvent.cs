using Domain.Common;
using Domain.Entities;

namespace Application.Features.AssignedProducts.Commands.CreateAssignedProduct
{
    public class AssignedProductCreatedEvent : BaseEvent
    {
        public AssignedProduct AssignedProduct { get; }
        public AssignedProductCreatedEvent(AssignedProduct assignedProduct)
        {
            AssignedProduct = assignedProduct;
        }
    }
}
