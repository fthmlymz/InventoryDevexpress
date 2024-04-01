using Domain.Common;
using Domain.Entities;

namespace Application.Features.Products.Commands.UpdateProduct
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
