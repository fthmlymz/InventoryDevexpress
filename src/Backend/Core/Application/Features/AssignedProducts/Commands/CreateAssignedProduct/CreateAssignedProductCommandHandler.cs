using Application.Features.AssignedProducts.Dtos;
using Application.Interfaces.Repositories;
using Domain.Entities;
using DotNetCore.CAP;
using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Features.AssignedProducts.Commands.CreateAssignedProduct;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.AssignedProducts.Commands.CreateAssignedProduct
{
    internal sealed class CreateAssignedProductCommandHandler : IRequestHandler<CreateAssignedProductCommand, Result<AssignedProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateAssignedProductCommandHandler> _logger;
        private readonly ICapPublisher _capPublisher;

        public CreateAssignedProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateAssignedProductCommandHandler> logger, ICapPublisher capPublisher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _capPublisher = capPublisher;
        }

        public async Task<Result<AssignedProductDto>> Handle(CreateAssignedProductCommand request, CancellationToken cancellationToken)
        {
            // ProductId'yi kullanarak Product tablosunda bir kayıt arayın
            var productExists = await _unitOfWork.Repository<Product>().Entities.AnyAsync(p => p.Id == request.ProductId);

            if (!productExists)
            {
                _logger.LogWarning($"No product found with Id: {request.ProductId}");
                throw new NotFoundExceptionCustom($"ProductId {request.ProductId} barkodlu ürün, ürünler tablosunda bulunamadı");
            }

            // ProductId'yi kullanarak AssignedProduct tablosunda bir kayıt ara
            var existingAssignedProduct = await _unitOfWork.Repository<AssignedProduct>().Entities.FirstOrDefaultAsync(ap => ap.ProductId == request.ProductId);

            if (existingAssignedProduct != null)
            {
                _logger.LogWarning($"An assigned product with ProductId {request.ProductId} already exists.");
                throw new ConflictExceptionCustom($"{request.ProductId} barkodlu ürün daha önce zimmetlenmiş");
            }


            var assignedProduct = request.Adapt<AssignedProduct>();
            await _unitOfWork.Repository<AssignedProduct>().AddAsync(assignedProduct);
            assignedProduct.AddDomainEvent(new AssignedProductCreatedEvent(assignedProduct));
            await _unitOfWork.SaveChangesAsync(cancellationToken);



            #region Zimmet bildirimi
            var createdAssignedDto = assignedProduct.Adapt<AssignedProductDto>();
            //createdAssignedDto.AssignedUserPhoto = null;
            createdAssignedDto.Email = request.Email;
            createdAssignedDto.FullName = request.FullName;
            createdAssignedDto.Barcode = request.Barcode;
            createdAssignedDto.ProductName = request.ProductName;

            await _capPublisher.PublishAsync("product.assigned.transaction", createdAssignedDto);
            #endregion


            return await Result<AssignedProductDto>.SuccessAsync(createdAssignedDto);
        }
    }
}
