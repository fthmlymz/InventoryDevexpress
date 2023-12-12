using DotNetCore.CAP;
using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Features.AssignedProducts.Dtos;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.AssignedProducts.Commands.AcceptRejectProduct
{
    internal class UpdatedAcceptRejectProductCommandHandler : IRequestHandler<UpdatedAcceptRejectProductCommand, Result<AssignedProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;
        private readonly ILogger<UpdatedAcceptRejectProductCommandHandler> _logger;

        public UpdatedAcceptRejectProductCommandHandler(IUnitOfWork unitOfWork, ICapPublisher capPublisher, ILogger<UpdatedAcceptRejectProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
            _logger = logger;
        }

        public async Task<Result<AssignedProductDto>> Handle(UpdatedAcceptRejectProductCommand request, CancellationToken cancellationToken)
        {
            var assignedProduct = await _unitOfWork.Repository<AssignedProduct>().GetByIdAsync(request.AssignedProductId.Value);
            if (assignedProduct == null)
            {
                _logger.LogWarning($"AssignedProduct Id not found: {request.AssignedProductId}", request.AssignedProductId);
                throw new NotFoundExceptionCustom($"{request.AssignedProductId} Id numaralı zimmet bulunamadı ve onay/red geri dönderildi.");
            }

            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var assignedProductProperty = assignedProduct.GetType().GetProperty(propertyName);
                    assignedProductProperty?.SetValue(assignedProduct, value);
                }
            }

            await _unitOfWork.Repository<AssignedProduct>().UpdateAsync(assignedProduct);
            assignedProduct.AddDomainEvent(new AcceptRejectProductEvent(assignedProduct));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await Result<AssignedProductDto>.SuccessAsync(assignedProduct.Adapt<AssignedProductDto>());
        }
    }
}


/*

         public async Task<Result<AssignedProductDto>> Handle(UpdatedAcceptRejectProductCommand request, CancellationToken cancellationToken)
        {
            var assignedProduct = await _unitOfWork.Repository<AssignedProduct>().GetByIdAsync(request.AssignedProductId.Value);

            if (assignedProduct == null)
            {
                _logger.LogWarning($"AssignedProduct Id not found: {request.AssignedProductId}", request.AssignedProductId);
                throw new NotFoundExceptionCustom($"{request.AssignedProductId} Id numaralı zimmet bulunamadı ve onay/red geri dönderildi.");
            }

            MapRequestToAssignedProduct(request, assignedProduct);

            UpdateApprovalStatus(assignedProduct);

            await UpdateAssignedProduct(assignedProduct, cancellationToken);

            return await Result<AssignedProductDto>.SuccessAsync(assignedProduct.Adapt<AssignedProductDto>());
        }

        private void MapRequestToAssignedProduct(UpdatedAcceptRejectProductCommand request, AssignedProduct assignedProduct)
        {
            var properties = request.GetType().GetProperties()
                .Where(propertyInfo => propertyInfo.GetValue(request) != null && !string.IsNullOrWhiteSpace(propertyInfo.GetValue(request).ToString()));

            foreach (var propertyInfo in properties)
            {
                var propertyName = propertyInfo.Name;
                var assignedProductProperty = assignedProduct.GetType().GetProperty(propertyName);
                assignedProductProperty?.SetValue(assignedProduct, propertyInfo.GetValue(request));
            }
        }

        private void UpdateApprovalStatus(AssignedProduct assignedProduct)
        {
            if (assignedProduct.ApprovalStatus == "Accepted" || assignedProduct.ApprovalStatus == "Rejected")
            {
                assignedProduct.ApprovalStatus = (assignedProduct.ApprovalStatus == "Accepted") ? "Onaylandı" : "Red Edildi";
            }
        }

        private async Task UpdateAssignedProduct(AssignedProduct assignedProduct, CancellationToken cancellationToken)
        {
            await _unitOfWork.Repository<AssignedProduct>().UpdateAsync(assignedProduct);
            assignedProduct.AddDomainEvent(new AcceptRejectProductEvent(assignedProduct));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
*/