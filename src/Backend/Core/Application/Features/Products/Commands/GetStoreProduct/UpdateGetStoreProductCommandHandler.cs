using Application.Interfaces.Repositories;
using Domain.Entities;
using Application.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;
using SharedLibrary.Common;

namespace Application.Features.Products.Commands.GetStoreProduct
{
    internal class UpdateGetStoreProductCommandHandler : IRequestHandler<UpdateGetStoreProductCommand, Result<Product>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateGetStoreProductCommandHandler> _logger;

        public UpdateGetStoreProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateGetStoreProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Product>> Handle(UpdateGetStoreProductCommand request, CancellationToken cancellationToken)
        {
            //Product check
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
            if (product == null)
            {
                _logger.LogWarning($"Product Id not found: {request.Id}", request.Id);
                throw new NotFoundExceptionCustom($"{request.Id} barkodlu ürün bulunamadı");
            }

            //Ürün depoda mı?
            if (string.Equals(product.Status, "Depoda", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"The product is already in stock: {request.Id}", request.Id);
                throw new BadRequestExceptionCustom($"{request.Id} numaralı ürün zaten depoda !!!");
            }

            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var productProperty = product.GetType().GetProperty(propertyName);
                    productProperty?.SetValue(product, value);
                }
            }
            product.Status = GenericConstantDefinitions.InStock;

            await _unitOfWork.Repository<Product>().UpdateAsync(product);
            product.AddDomainEvent(new GetStoreProductUpdatedEvent(product));
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return await Result<Product>.SuccessAsync(product);
        }
    }
}
