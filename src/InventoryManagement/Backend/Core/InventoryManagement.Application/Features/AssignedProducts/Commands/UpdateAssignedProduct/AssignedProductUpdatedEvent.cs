using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.AssignedProducts.Commands.UpdateAssignedProduct
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
