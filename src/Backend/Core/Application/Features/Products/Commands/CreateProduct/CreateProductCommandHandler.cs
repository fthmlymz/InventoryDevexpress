using Application.Interfaces.Repositories;
using Domain.Entities;
using DotNetCore.CAP;
using Application.Common.Exceptions;
using Application.Features.Products.GeneralDtos;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.Products.Commands.CreateProduct
{
    internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreatedProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateProductCommandHandler> _logger;
        private readonly ICapPublisher _capPublisher;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductCommandHandler> logger, ICapPublisher capPublisher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _capPublisher = capPublisher;
        }

        public async Task<Result<CreatedProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Barcode daha önce kaydedilmiş mi?
            var existingProduct = _unitOfWork.Repository<Product>().Entities.FirstOrDefault(x => x.Barcode == request.Barcode);
            if (existingProduct != null)
            {
                _logger.LogWarning($"Barcode already exists for product: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} için kaydedilmek istenen barkod zaten mevcut");
            }


            //Şirket kayıtlı mı ?
            var companyExists = _unitOfWork.Repository<Company>().Entities.SingleOrDefault(x => x.Id == request.CompanyId);
            if (companyExists == null)
            {
                _logger.LogWarning($"Company Id not found for product record: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} için kayıt edilecek şirket bilgisi bulunamadı");
            }


            var product = request.Adapt<Product>();
            await _unitOfWork.Repository<Product>().AddAsync(product);
            product.AddDomainEvent(new ProductCreatedEvent(product));
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            var createdProductDto = product.Adapt<CreatedProductDto>();


            #region ProductMovement ekle
            var movement = new CreatedProductMovementDto
            {
                ProductId = createdProductDto.Id,
                Description = $"{request.CreatedBy} tarafından oluşturuldu",
                CreatedBy = createdProductDto.CreatedBy,
                CreatedDate = createdProductDto.CreatedDate,
                CreatedUserId = createdProductDto.CreatedUserId,
                MovementDate = DateTime.Now
            };
            var movementDto = movement.Adapt<ProductMovement>();
            await _unitOfWork.Repository<ProductMovement>().AddAsync(movementDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            #endregion

            //Ürün kayıt edildiğini publish et
            await _capPublisher.PublishAsync("product.created.transaction", createdProductDto);

            return await Result<CreatedProductDto>.SuccessAsync(createdProductDto);
        }
    }
}
