using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;

namespace InventoryManagement.Application.Features.Products.Commands.ProductOperations
{
    public class ProductOperationsUpdatedEventHandler : INotificationHandler<ProductOperationsUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductOperationsUpdatedEventHandler> _logger;

        public ProductOperationsUpdatedEventHandler(IUnitOfWork unitOfWork, ILogger<ProductOperationsUpdatedEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(ProductOperationsUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var updatedProduct = notification.UpdateProductOperationsCommand;

            #region Product Movement Add
            var typeOfOperations = updatedProduct?.TypeOfOperations?.Split(',').Select(s => s.Trim());
            var descriptions = new List<string>();
            foreach (var operation in typeOfOperations)
            {
                string description;

                switch (operation)
                {
                    case GenericConstantDefinitions.Transfer:
                        description = $"{updatedProduct?.UpdatedBy} tarafından, {updatedProduct?.SenderCompanyName} --> {updatedProduct?.RecipientCompanyName} Transfer işlemi yapıldı.";
                        break;
                    case GenericConstantDefinitions.Accepted:
                        description = $"{updatedProduct?.UpdatedBy} tarafından, Depoya alma işlemi yapıldı.";
                        break;
                    case GenericConstantDefinitions.Rejected:
                        description = $"{updatedProduct?.UpdatedBy} tarafından, {updatedProduct?.Id} Red Edildi.";
                        break;
                    case GenericConstantDefinitions.ReturnIt:
                        description = $"{updatedProduct?.UpdatedBy} tarafından, {updatedProduct?.Id} ürün İade Edildi.";
                        break;
                    default:
                        description = $"{updatedProduct?.UpdatedBy} tarafından, Bilinmeyen bir işlem yapıldı.";
                        _logger.LogWarning($"An unknown operation was committed by {updatedProduct?.UpdatedBy}");
                        break;
                }
                descriptions.Add(description);
            }

            var combinedDescription = string.Join(", ", descriptions);

            var productMovement = new ProductMovement
            {
                MovementDate = DateTime.Now,
                Description = combinedDescription,
                ProductId = updatedProduct.Id,
                CreatedBy = updatedProduct.UpdatedBy,
                CreatedUserId = updatedProduct.UpdatedUserId,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Repository<ProductMovement>().AddAsync(productMovement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            #endregion
        }
    }
}
