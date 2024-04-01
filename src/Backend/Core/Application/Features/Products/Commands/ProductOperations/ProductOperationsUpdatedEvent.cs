using Domain.Common;

namespace Application.Features.Products.Commands.ProductOperations
{
    public class ProductOperationsUpdatedEvent : BaseEvent
    {
        public UpdateProductOperationsCommand UpdateProductOperationsCommand { get; set; }
        public ProductOperationsUpdatedEvent(UpdateProductOperationsCommand updateProductOperationsCommand)
        {
            UpdateProductOperationsCommand = updateProductOperationsCommand;
        }
    }
}
