using Domain.Common;
using Domain.Entities;

namespace Application.Features.AssignedProducts.Commands.AcceptRejectProduct
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
