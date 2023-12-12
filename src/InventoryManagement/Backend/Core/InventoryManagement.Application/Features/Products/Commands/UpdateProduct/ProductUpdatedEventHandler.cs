using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Products.Commands.UpdateProduct
{
    public class ProductUpdatedEventHandler : INotificationHandler<ProductUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductUpdatedEventHandler> _logger;

        public ProductUpdatedEventHandler(IUnitOfWork unitOfWork, ILogger<ProductUpdatedEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var updatedProduct = notification.Product;

            #region Product Movement Add
            var productMovement = new ProductMovement
            {
                MovementDate = DateTime.Now,
                Description = $"{updatedProduct.UpdatedBy} tarafından Ürün güncelleme işlemi yapıldı.",
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



            //#region AssignedProduct
            //if (updatedProduct.Status == "Depoda")
            //{
            //    var assignedProduct = _unitOfWork.Repository<AssignedProduct>().Entities.FirstOrDefault(ap => ap.ProductId == updatedProduct.Id);

            //    if (assignedProduct != null)
            //    {
            //        assignedProduct.AssignedUserId = null;
            //        assignedProduct.AssignedUserName = null;
            //        assignedProduct.AssignedUserPhoto = null;
            //        assignedProduct.UpdatedBy = assignedProduct.UpdatedBy;
            //        assignedProduct.UpdatedUserId = assignedProduct.UpdatedUserId;
            //        await _unitOfWork.Repository<AssignedProduct>().UpdateAsync(assignedProduct);
            //        await _unitOfWork.SaveChangesAsync(cancellationToken);
            //    }
            //}
            //#endregion
        }
    }
}
