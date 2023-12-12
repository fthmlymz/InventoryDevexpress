using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.AssignedProducts.Commands.CreateAssignedProduct
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
