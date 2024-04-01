using Domain.Common;
using Domain.Entities;

namespace Application.Features.Products.Commands.CreateProduct
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
