using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;

namespace InventoryManagement.Application.Features.Products.Commands.GetStoreProduct
{
    public class GetStoreProductUpdatedEventHandler : INotificationHandler<GetStoreProductUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetStoreProductUpdatedEventHandler> _logger;

        public GetStoreProductUpdatedEventHandler(IUnitOfWork unitOfWork, ILogger<GetStoreProductUpdatedEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(GetStoreProductUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var updatedProduct = notification.Product;

            #region Product Movement Add
            var productMovement = new ProductMovement
            {
                MovementDate = DateTime.Now,
                Description = $"{updatedProduct.UpdatedBy} tarafından Depoya alma işlemi yapıldı",
                ProductId = updatedProduct.Id,
                UpdatedBy = updatedProduct.UpdatedBy,
                UpdatedUserId = updatedProduct.UpdatedUserId,
                UpdatedDate = DateTime.Now,
                CreatedBy = updatedProduct.CreatedBy,
                CreatedUserId = updatedProduct.CreatedUserId,
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.Repository<ProductMovement>().AddAsync(productMovement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            #endregion




            #region AssignedProduct
            if (updatedProduct.Status == GenericConstantDefinitions.InStock)
            { //GetById yapılacak, firstasync kaldırılacak
                var assignedProduct = _unitOfWork.Repository<AssignedProduct>().Entities.FirstOrDefault(ap => ap.ProductId == updatedProduct.Id);

                if (assignedProduct != null)
                {
                    assignedProduct.AssignedUserId = null;
                    assignedProduct.AssignedUserName = null;
                    assignedProduct.AssignedUserPhoto = null;
                    assignedProduct.ApprovalStatus = null;
                    assignedProduct.FullName = null;
                    assignedProduct.UpdatedBy = assignedProduct.UpdatedBy;
                    assignedProduct.UpdatedUserId = assignedProduct.UpdatedUserId;
                    await _unitOfWork.Repository<AssignedProduct>().UpdateAsync(assignedProduct);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            #endregion
        }
    }
}
