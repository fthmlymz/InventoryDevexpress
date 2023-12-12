using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Products.Queries.GetProductListWithPaginationQuery
{
    public sealed record GetProductWithPaginationQuery : IRequest<PaginatedResult<GetProductWithPaginationDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public GetProductWithPaginationQuery() { }
        public GetProductWithPaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    internal class GetProductQueryHandler : IRequestHandler<GetProductWithPaginationQuery, PaginatedResult<GetProductWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetProductQueryHandler> _logger;

        public GetProductQueryHandler(IUnitOfWork unitOfWork, ILogger<GetProductQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaginatedResult<GetProductWithPaginationDto>> Handle(GetProductWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Product>().Entities
                .OrderBy(x => x.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);

            var totalCount = await _unitOfWork.Repository<Product>().Entities.CountAsync(cancellationToken);

            var dtos = await query.Select(p => new GetProductWithPaginationDto
            {
                Id = p.Id,
                Name = p.Name,
                Barcode = p.Barcode,
                CompanyId = p.CompanyId,
                CreatedBy = p.CreatedBy,
                CreatedDate = p.CreatedDate,
                CreatedUserId = p.CreatedUserId,
                DataClass = p.DataClass,
                Imei = p.Imei,
                InvoiceDate = p.InvoiceDate,
                Mac = p.Mac,
                ProductDate = p.ProductDate,
                SerialNumber = p.SerialNumber,
                Status = p.Status,
                PurchaseDate = p.PurchaseDate,

                UpdatedBy = p.UpdatedBy,
                UpdatedDate = p.UpdatedDate,
                UpdatedUserId = p.UpdatedUserId,

                BrandId = p.BrandId,
                CategoryId = p.CategoryId,
                CategorySubId = p.CategorySubId,
                ModelId = p.ModelId,

                CategoryName = p.Category != null ? p.Category.Name : null,
                CategorySubName = p.CategorySub != null ? p.CategorySub.Name : null,
                BrandName = p.Brand != null ? p.Brand.Name : null,
                ModelName = p.Model != null ? p.Model.Name : null,

                CompanyName = p.Company != null ? p.Company.Name : null,

                AssignedUserName = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault().AssignedUserName,
                AssignedUserId = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault().AssignedUserId,
                FullName = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault().FullName,
                ApprovalStatus = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault().ApprovalStatus,

                ProductId = p.Id
            }).ToListAsync(cancellationToken);

            return new PaginatedResult<GetProductWithPaginationDto>(true, dtos, count: totalCount, pageNumber: request.PageNumber, pageSize: request.PageSize);
        }
    }
}



//performans kontrolleri yapılacak
/*public async Task<PaginatedResult<GetProductWithPaginationDto>> Handle(GetProductWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Product>().Entities
                                    .OrderBy(x => x.Name)
                                    .Skip((request.PageNumber - 1) * request.PageSize)
                                    .Take(request.PageSize);

            var totalCount = await _unitOfWork.Repository<Product>().Entities.CountAsync(cancellationToken);

            var productQuery = query.Select(p => new
            {
                Product = p,
                LatestAssignedProduct = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault()
            });

            var dtos = await productQuery.Select(p => new GetProductWithPaginationDto
            {
                Id = p.Product.Id,
                Name = p.Product.Name,
                Barcode = p.Product.Barcode,
                CompanyId = p.Product.CompanyId,
                CreatedBy = p.Product.CreatedBy,
                CreatedDate = p.Product.CreatedDate,
                CreatedUserId = p.Product.CreatedUserId,
                DataClass = p.Product.DataClass,
                Imei = p.Product.Imei,
                InvoiceDate = p.Product.InvoiceDate,
                Mac = p.Product.Mac,
                ProductDate = p.Product.ProductDate,
                SerialNumber = p.Product.SerialNumber,
                Status = p.Product.Status,
                UpdatedBy = p.Product.UpdatedBy,
                UpdatedDate = p.Product.UpdatedDate,
                UpdatedUserId = p.Product.UpdatedUserId,

                CategoryName = p.Product.Category.Name,
                CategorySubName = p.Product.CategorySub.Name,
                BrandName = p.Product.Brand.Name,
                ModelName = p.Product.Model.Name,

                AssignedUserName = p.LatestAssignedProduct.AssignedUserName,
                AssignedUserId = p.LatestAssignedProduct.AssignedUserId,
                FullName = p.LatestAssignedProduct.FullName,
                ApprovalStatus = p.LatestAssignedProduct.ApprovalStatus,

                ProductId = p.Product.Id
            }).ToListAsync(cancellationToken);
            return new PaginatedResult<GetProductWithPaginationDto>(true, dtos, count: totalCount, pageNumber: request.PageNumber, pageSize: request.PageSize);
        }*/



/*var query = _unitOfWork.Repository<Product>().Entities
                .Include(p => p.Category)
                .Include(p => p.CategorySub)
                .Include(p => p.Brand)
                .Include(p => p.Model)
                .OrderBy(x => x.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);

            var totalCount = await _unitOfWork.Repository<Product>().Entities.CountAsync(cancellationToken);

            var products = await query
                .Select(p => new
                {
                    Product = p,
                    LatestAssignedProduct = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            var dtos = products.Select(p => new GetProductWithPaginationDto
            {
                Id = p.Product.Id,
                Name = p.Product.Name,
                Barcode = p.Product.Barcode,
                CompanyId = p.Product.CompanyId,
                CreatedBy = p.Product.CreatedBy,
                CreatedDate = p.Product.CreatedDate,
                CreatedUserId = p.Product.CreatedUserId,
                DataClass = p.Product.DataClass,
                Imei = p.Product.Imei,
                InvoiceDate = p.Product.InvoiceDate,
                Mac = p.Product.Mac,
                ProductDate = p.Product.ProductDate,
                SerialNumber = p.Product.SerialNumber,
                Status = p.Product.Status,
                UpdatedBy = p.Product.UpdatedBy,
                UpdatedDate = p.Product.UpdatedDate,
                UpdatedUserId = p.Product.UpdatedUserId,


                CategoryId = p.Product.CategoryId,
                CategoryName = p.Product.Category != null ? p.Product.Category.Name : null,
                CategorySubId = p.Product.CategorySubId,
                CategorySubName = p.Product.CategorySub != null ? p.Product.CategorySub.Name : null,
                BrandId = p.Product.BrandId,
                BrandName = p.Product.Brand != null ? p.Product.Brand.Name : null,
                ModelId = p.Product.ModelId,
                ModelName = p.Product.Model != null ? p.Product.Model.Name : null,


                AssignedUserName = p.LatestAssignedProduct?.AssignedUserName,
                AssignedUserId = p.LatestAssignedProduct?.AssignedUserId,
                AssignedUserPhoto = p.LatestAssignedProduct?.AssignedUserPhoto,
                FullName = p.LatestAssignedProduct?.FullName,
                ApprovalStatus = p.LatestAssignedProduct?.ApprovalStatus,

                ProductId = p.Product.Id
            }).ToList();

            return new PaginatedResult<GetProductWithPaginationDto>(true, dtos, count: totalCount, pageNumber: request.PageNumber, pageSize: request.PageSize);*/


/*Performans açısından en iyi bu
 * public async Task<PaginatedResult<GetProductWithPaginationDto>> Handle(GetProductWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Product>().Entities.OrderBy(x => x.Name);

            var totalCount = await query.CountAsync(cancellationToken);

            var products = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new GetProductWithPaginationDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Barcode = p.Barcode,
                    CompanyId = p.CompanyId,
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedDate,
                    CreatedUserId = p.CreatedUserId,
                    DataClass = p.DataClass,
                    Imei = p.Imei,
                    InvoiceDate = p.InvoiceDate,
                    Mac = p.Mac,
                    ProductDate = p.ProductDate,
                    SerialNumber = p.SerialNumber,
                    Status = p.Status,
                    UpdatedBy = p.UpdatedBy,
                    UpdatedDate = p.UpdatedDate,
                    UpdatedUserId = p.UpdatedUserId,

                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    CategorySubId = p.CategorySubId,
                    CategorySubName = p.CategorySub != null ? p.CategorySub.Name : null,
                    BrandId = p.BrandId,
                    BrandName = p.Brand != null ? p.Brand.Name : null,
                    ModelId = p.ModelId,
                    ModelName = p.Model != null ? p.Model.Name : null,

                    AssignedUserName = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault() != null ? p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault().AssignedUserName : null,
                    AssignedUserId = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault() != null ? p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault().AssignedUserId : null,
                    AssignedUserPhoto = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault() != null ? p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault().AssignedUserPhoto : null,
                    FullName = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault() != null ? p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault().FullName : null,
                    ApprovalStatus = p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault() != null ? p.AssignedProducts.OrderByDescending(ap => ap.CreatedDate).FirstOrDefault().ApprovalStatus : null,

                    ProductId = p.Id
                })
                .ToListAsync(cancellationToken);

            return new PaginatedResult<GetProductWithPaginationDto>(true, products, count: totalCount, pageNumber: request.PageNumber, pageSize: request.PageSize);
        }*/

