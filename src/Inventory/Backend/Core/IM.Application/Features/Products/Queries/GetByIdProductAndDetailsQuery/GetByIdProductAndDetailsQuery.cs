using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Features.Products.Commands.ProductOperations;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;

namespace InventoryManagement.Application.Features.Products.Queries.GetByIdProductAndDetailsQuery
{
    public sealed record GetByIdProductAndDetailsQuery : IRequest<Result<List<GetByIdProductAndDetailsDto>>>
    {
        public int Id { get; set; }
        public GetByIdProductAndDetailsQuery() { }
        public GetByIdProductAndDetailsQuery(int id)
        {
            Id = id;
        }
    }


    internal class GetByIdProductAndDetailsHandler : IRequestHandler<GetByIdProductAndDetailsQuery, Result<List<GetByIdProductAndDetailsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEasyCacheService _easyCacheService;
        private readonly ILogger<GetByIdProductAndDetailsHandler> _logger;
        private readonly IWorkflowRepository _workflowRepository;

        public GetByIdProductAndDetailsHandler(IUnitOfWork unitOfWork, IEasyCacheService easyCacheService, ILogger<GetByIdProductAndDetailsHandler> logger, IWorkflowRepository workflowRepository)
        {
            _unitOfWork = unitOfWork;
            _easyCacheService = easyCacheService;
            _logger = logger;
            _workflowRepository = workflowRepository;
        }


        public async Task<Result<List<GetByIdProductAndDetailsDto>>> Handle(GetByIdProductAndDetailsQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
            if (product == null)
            {
                _logger.LogInformation($"Requested company not found: {request.Id}");
                throw new NotFoundExceptionCustom($"{request.Id} numaralı ürün bulunamadı");
            }

            var productMovements = await _unitOfWork.Repository<ProductMovement>()
                .Entities
                .Where(pm => pm.ProductId == request.Id)
                .OrderByDescending(pm => pm.CreatedDate)
                .ToListAsync();

            var assignedProducts = await _unitOfWork.Repository<AssignedProduct>()
                .Entities
                .Where(ap => ap.ProductId == request.Id)
                .OrderByDescending(ap => ap.CreatedDate)
                .ToListAsync();


            var companyNames = (await _unitOfWork.Repository<Company>().Entities
                .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken));


            // Marka ve model nesnelerini alın
            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(product.BrandId.Value);
            var model = await _unitOfWork.Repository<Model>().GetByIdAsync(product.ModelId.Value);
            
            if (brand == null || model == null)
            {
                // Marka veya model bulunamazsa ilgili hata işlenmeli
                throw new NotFoundExceptionCustom($"Marka veya model bulunamadı: BrandId: {product.BrandId.Value} ModelId: {product.ModelId.Value}");
            }


            ProductTransferQuerDto productTransferDto = null;
            if (!string.IsNullOrEmpty(product?.WorkflowId))
            {
                var transferQueryResult = _workflowRepository?.GetWorkflowInstance(product?.WorkflowId)?.Result;
                if (transferQueryResult?.Data == null)
                {
                    _logger.LogInformation($"Workflow not found for product {request.Id}");
                    throw new NotFoundExceptionCustom($"{request.Id} Id numaralı ürün için iş akışı bulunamadı");
                }

                var transferQueryDto = transferQueryResult?.Data as UpdateProductOperationsCommand;
                productTransferDto = new ProductTransferQuerDto
                {
                    SenderUserName = transferQueryDto?.SenderUserName,
                    SenderEmail = transferQueryDto?.SenderEmail,
                    SenderCompanyName = transferQueryDto?.SenderCompanyName,
                    RecipientUserName = transferQueryDto?.RecipientUserName,
                    RecipientCompanyId = transferQueryDto?.RecipientCompanyId,
                    RecipientCompanyName = transferQueryDto?.RecipientCompanyName,
                    RecipientEmail = transferQueryDto?.RecipientEmail,
                };
            }


            var dto = new GetByIdProductAndDetailsDto
            {
                Id = product.Id,
                Name = product.Name,
                Barcode = product.Barcode,
                SerialNumber = product.SerialNumber,
                Imei = product.Imei,
                Mac = product.Mac,
                DataClass = product.DataClass,
                Status = product.Status,
                PurchaseDate = product.PurchaseDate,
                ProductDate = product.ProductDate,
                InvoiceDate = product.InvoiceDate,
                CompanyId = product.CompanyId,
                CreatedBy = product.CreatedBy,
                CreatedDate = product.CreatedDate,
                CreatedUserId = product.CreatedUserId,
                UpdatedBy = product.UpdatedBy,
                UpdatedDate = product.UpdatedDate,
                UpdatedUserId = product.UpdatedUserId,
                BrandId = product.BrandId,
                BrandName = brand.Name,
                CategoryId = product.CategoryId,
                CategorySubId = product.CategorySubId,
                ModelId = product.ModelId,
                ModelName = model.Name,
                CompanyName = companyNames.GetValueOrDefault(product.CompanyId),

                ProductTransfers = productTransferDto != null ? new List<ProductTransferQuerDto> { productTransferDto } : new List<ProductTransferQuerDto> { productTransferDto },
                ProductMovements = productMovements?.Select(pm => new ProductMovementQueryDto
                {
                    Id = pm.Id,
                    CreatedBy = pm.CreatedBy,
                    UpdatedUserId = pm.UpdatedUserId,
                    UpdatedDate = pm.UpdatedDate,
                    UpdatedBy = pm.UpdatedBy,
                    ProductId = pm.ProductId,
                    CreatedUserId = pm.CreatedUserId,
                    CreatedDate = pm.CreatedDate,
                    MovementDate = pm.MovementDate,
                    Description = pm.Description
                }).ToList(),
                AssignedProducts = assignedProducts?.Select(ap => new AssignedProductQueryDto
                {
                    ProductId = ap.ProductId,
                    CreatedBy = ap.CreatedBy,
                    CreatedDate = ap.CreatedDate,
                    CreatedUserId = ap.CreatedUserId,
                    Id = ap.Id,
                    UpdatedBy = ap.UpdatedBy,
                    UpdatedDate = ap.UpdatedDate,
                    UpdatedUserId = ap.UpdatedUserId,
                    AssignedUserName = ap.AssignedUserName,
                    AssignedUserId = ap.AssignedUserId,
                    AssignedUserPhoto = ap.AssignedUserPhoto,
                    FullName = ap.FullName,
                    ApprovalStatus = ap.ApprovalStatus,
                }).ToList()
            };
            return await Result<List<GetByIdProductAndDetailsDto>>.SuccessAsync(new List<GetByIdProductAndDetailsDto> { dto });
        }
    }
}
