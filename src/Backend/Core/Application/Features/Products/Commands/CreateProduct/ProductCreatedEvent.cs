using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Products.Commands.CreateProduct
{
    public class ProductCreatedEvent : BaseEvent
    {
        public Product Product { get; }
        public ProductCreatedEvent(Product product)
        {
            Product = product;
        }
    }
}
