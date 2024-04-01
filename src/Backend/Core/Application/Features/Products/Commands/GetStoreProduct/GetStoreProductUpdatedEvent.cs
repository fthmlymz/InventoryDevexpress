using Domain.Common;
using Domain.Entities;

namespace Application.Features.Products.Commands.GetStoreProduct
{
    public class GetStoreProductUpdatedEvent : BaseEvent
    {
        public Product Product { get; set; }
        public GetStoreProductUpdatedEvent(Product product)
        {
            Product = product;
        }
    }
}
