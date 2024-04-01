using Domain.Common;
using Domain.Entities;

namespace Application.Features.AssignedProducts.Commands.UpdateAssignedProduct
{
    public class AssignedProductUpdatedEvent : BaseEvent
    {
        public AssignedProduct AssignedProduct { get; set; }

        public AssignedProductUpdatedEvent(AssignedProduct assignedProduct)
        {
            AssignedProduct = assignedProduct;
        }
    }
}
