using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Products.Commands.UpdateProduct
{
    public class ProductUpdatedEvent : BaseEvent
    {
        public Product Product { get; set; }

        public ProductUpdatedEvent(Product product)
        {
            Product = product;
        }
    }
}
