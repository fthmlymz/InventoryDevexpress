using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Products.Commands.DeleteProduct
{
    public class ProductDeletedEvent : BaseEvent
    {
        public Product Product { get; set; }

        public ProductDeletedEvent(Product product)
        {
            Product = product;
        }
    }
}
