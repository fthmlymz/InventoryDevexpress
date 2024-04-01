using Domain.Common;
using Domain.Entities;

namespace Application.Features.Products.Commands.DeleteProduct
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
