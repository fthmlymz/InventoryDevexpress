﻿using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<Product>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<Result<Product>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            //Product check
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
            if (product == null)
            {
                _logger.LogWarning($"Product Id not found: {request.Id}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} isimli ürün bulunamadı");
            }

            //Product kayıt edilecek company bilgisi
            var company = _unitOfWork.Repository<Company>().Entities.FirstOrDefault(x => x.Id == request.CompanyId);
            if (company == null)
            {
                _logger.LogWarning($"Company ID not found: {request.Name}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} için kayıt edilecek şirket bilgisi bulunamadu");
            }


            // Product bilgisini güncelle
            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var productProperty = product.GetType().GetProperty(propertyName);
                    productProperty.SetValue(product, value);
                }
            }

            await _unitOfWork.Repository<Product>().UpdateAsync(product);
            product.AddDomainEvent(new ProductUpdatedEvent(product));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await Result<Product>.SuccessAsync(product);
        }
    }
}