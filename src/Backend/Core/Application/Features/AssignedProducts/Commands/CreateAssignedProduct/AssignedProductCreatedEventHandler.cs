using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.AssignedProducts.Commands.CreateAssignedProduct
{
    public class AssignedProductCreatedEventHandler : INotificationHandler<AssignedProductCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AssignedProductCreatedEventHandler> _logger;

        public AssignedProductCreatedEventHandler(IUnitOfWork unitOfWork, ILogger<AssignedProductCreatedEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(AssignedProductCreatedEvent notification, CancellationToken cancellationToken)
        {
            var createdAssignedProduct = notification.AssignedProduct;

            var existingProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(createdAssignedProduct.ProductId);

            #region
            if (existingProduct != null)
            {
                existingProduct.CreatedBy = createdAssignedProduct.CreatedBy;
                existingProduct.Status = "Zimmetlendi";
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            #endregion


            #region Product Assigned
            if (existingProduct != null)
            {
                existingProduct.CreatedBy = createdAssignedProduct.CreatedBy;
                existingProduct.Status = "Zimmetlendi";
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var productMovement = new ProductMovement
                {
                    CreatedBy = createdAssignedProduct.CreatedBy,
                    CreatedUserId = createdAssignedProduct.CreatedUserId,
                    CreatedDate = createdAssignedProduct.CreatedDate,
                    MovementDate = DateTime.Now,
                    Description = $"{createdAssignedProduct.CreatedBy} tarafından, {existingProduct.Barcode} barkodlu ürün {createdAssignedProduct.FullName} - ({createdAssignedProduct.AssignedUserName}) üzerine zimmetlendi", // Hareket açıklaması veya başka bir bilgi
                    ProductId = createdAssignedProduct.ProductId
                };
                await _unitOfWork.Repository<ProductMovement>().AddAsync(productMovement);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            #endregion
        }
    }
}
