using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.AssignedProducts.Commands.UpdateAssignedProduct
{
    public class AssignedProductUpdatedEventHandler : INotificationHandler<AssignedProductUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AssignedProductUpdatedEventHandler> _logger;

        public AssignedProductUpdatedEventHandler(IUnitOfWork unitOfWork, ILogger<AssignedProductUpdatedEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(AssignedProductUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var updatedAssignedProduct = notification.AssignedProduct;

            #region Product Movement Add
            var productMovement = new ProductMovement
            {
                MovementDate = DateTime.Now,
                Description = $"{updatedAssignedProduct.UpdatedBy} tarafından, {updatedAssignedProduct.FullName}({updatedAssignedProduct.AssignedUserName}) için Zimmet işlemi yapıldı",
                ProductId = updatedAssignedProduct.ProductId, //Id değiştirildi kontrol edilecek
                UpdatedBy = updatedAssignedProduct.UpdatedBy,
                UpdatedUserId = updatedAssignedProduct.UpdatedUserId,
                UpdatedDate = DateTime.Now,
                CreatedBy = updatedAssignedProduct.CreatedBy,
                CreatedUserId = updatedAssignedProduct.CreatedUserId,
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.Repository<ProductMovement>().AddAsync(productMovement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            #endregion



            #region Product "Status" change
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(updatedAssignedProduct.ProductId);
            if (product != null)
            {
                product.Status = "Zimmetlendi";
                product.UpdatedBy = updatedAssignedProduct.UpdatedBy;
                await _unitOfWork.Repository<Product>().UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            #endregion
        }
    }
}
