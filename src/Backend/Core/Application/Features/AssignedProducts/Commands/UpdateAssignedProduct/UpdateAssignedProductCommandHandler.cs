using Application.Features.AssignedProducts.Dtos;
using Application.Interfaces.Repositories;
using Domain.Entities;
using DotNetCore.CAP;
using InventoryManagement.Application.Common.Exceptions;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.AssignedProducts.Commands.UpdateAssignedProduct
{
    internal class UpdateAssignedProductCommandHandler : IRequestHandler<UpdateAssignedProductCommand, Result<AssignedProduct>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateAssignedProductCommandHandler> _logger;
        private readonly ICapPublisher _capPublisher;

        public UpdateAssignedProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateAssignedProductCommandHandler> logger, ICapPublisher capPublisher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _capPublisher = capPublisher;
        }

        public async Task<Result<AssignedProduct>> Handle(UpdateAssignedProductCommand request, CancellationToken cancellationToken)
        {
            var assignedProduct = await _unitOfWork.Repository<AssignedProduct>().GetByIdAsync(request.Id.Value);
            if (assignedProduct == null)
            {
                _logger.LogWarning($"AssignedProduct Id not found: {request.Id}", request.Id);
                throw new NotFoundExceptionCustom($"{request.Id} Id numaralı zimmet kaydı bulunamadı");
            }

            // AssignedUserName alanının null olmadığını kontrol ediyoruz.
            if (!string.IsNullOrEmpty(assignedProduct.AssignedUserName))
            {
                _logger.LogWarning($"Barcoded products have been previously assined : {request.ProductId}", request.ProductId);
                throw new BadRequestExceptionCustom($"{request.ProductId} barkodlu ürün daha önce zimmetlenmiş");
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
            assignedProduct.AddDomainEvent(new AssignedProductUpdatedEvent(assignedProduct));
            await _unitOfWork.SaveChangesAsync(cancellationToken);



            #region Zimmet Bildirimi
            var updatedAssignedDto = assignedProduct.Adapt<AssignedProductDto>();
            updatedAssignedDto.Email = request.Email;
            updatedAssignedDto.Barcode = request.Barcode;
            //updatedAssignedDto.AssignedUserPhoto = null;
            await _capPublisher.PublishAsync("product.assigned.transaction", updatedAssignedDto);
            #endregion



            return await Result<AssignedProduct>.SuccessAsync(assignedProduct);
        }
    }
}
