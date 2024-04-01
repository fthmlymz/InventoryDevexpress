using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.AssignedProducts.Commands.AcceptRejectProduct
{
    public class AcceptRejectProductEventHandler : INotificationHandler<AcceptRejectProductEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AcceptRejectProductEventHandler> _logger;

        public AcceptRejectProductEventHandler(IUnitOfWork unitOfWork, ILogger<AcceptRejectProductEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(AcceptRejectProductEvent notification, CancellationToken cancellationToken)
        {
            var assignedProduct = notification.AssignedProduct;

            #region Product Movement Add
            string approvalStatusDescription = assignedProduct.ApprovalStatus == "Onaylandı" ? "Onaylandı" : "Red Edildi";

            var productMovement = new ProductMovement
            {
                MovementDate = DateTime.Now,
                Description = $"{assignedProduct.AssignedUserName} tarafından {approvalStatusDescription}",
                ProductId = assignedProduct.ProductId,
                UpdatedBy = assignedProduct.AssignedUserName,
                UpdatedUserId = assignedProduct.AssignedUserName,
                UpdatedDate = DateTime.Now,
                CreatedBy = assignedProduct.AssignedUserName,
                CreatedUserId = assignedProduct.AssignedUserName,
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.Repository<ProductMovement>().AddAsync(productMovement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            #endregion
        }


    }
}
