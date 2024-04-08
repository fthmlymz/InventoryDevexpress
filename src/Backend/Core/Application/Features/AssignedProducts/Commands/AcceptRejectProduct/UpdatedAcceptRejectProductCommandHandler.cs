using Application.Features.AssignedProducts.Dtos;
using Application.Interfaces.Repositories;
using Domain.Entities;
using DotNetCore.CAP;
using Application.Common.Exceptions;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.AssignedProducts.Commands.AcceptRejectProduct
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
