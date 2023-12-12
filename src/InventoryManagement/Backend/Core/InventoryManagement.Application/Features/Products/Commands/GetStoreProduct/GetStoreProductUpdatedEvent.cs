using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Products.Commands.GetStoreProduct
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
