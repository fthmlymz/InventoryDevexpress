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
