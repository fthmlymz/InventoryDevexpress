using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Products.Commands.ProductOperations
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
