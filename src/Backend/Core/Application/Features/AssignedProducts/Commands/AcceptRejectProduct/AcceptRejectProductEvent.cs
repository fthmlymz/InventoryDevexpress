using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.AssignedProducts.Commands.AcceptRejectProduct
{
    public class AcceptRejectProductEvent : BaseEvent
    {
        public AssignedProduct AssignedProduct { get; set; }

        public AcceptRejectProductEvent(AssignedProduct assignedProduct)
        {
            AssignedProduct = assignedProduct;
        }

       
    }
}
