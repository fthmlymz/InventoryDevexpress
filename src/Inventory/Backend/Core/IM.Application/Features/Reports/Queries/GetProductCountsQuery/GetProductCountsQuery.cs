using InventoryManagement.Application.Features.Products.Queries.GetByIdProductAndDetailsQuery;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Reports.Queries.GetProductCountsQuery
{
    public sealed record GetProductCountsQuery() : IRequest<Result<List<CategoryProductCountsDto>>>;


    internal class GetProductCountsQueryHandler : IRequestHandler<GetProductCountsQuery, Result<List<CategoryProductCountsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEasyCacheService _easyCacheService;
        private readonly ILogger<GetByIdProductAndDetailsHandler> _logger;
        public GetProductCountsQueryHandler(IUnitOfWork unitOfWork, IEasyCacheService easyCacheService, ILogger<GetByIdProductAndDetailsHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _easyCacheService = easyCacheService;
            _logger = logger;
        }

        public async Task<Result<List<CategoryProductCountsDto>>> Handle(GetProductCountsQuery request, CancellationToken cancellationToken)
        {
            var productCounts = _unitOfWork.Repository<Product>()
                .Entities
                .GroupBy(product => new
                {
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,
                    CategorySubId = product.CategorySubId,
                    CategorySubName = product.CategorySub.Name,
                    BrandId = product.BrandId,
                    BrandName = product.Brand.Name,
                    ModelId = product.ModelId,
                    ModelName = product.Model.Name
                })
                .Select(group => new CategoryProductCountsDto
                {
                    CategoryId = group.Key.CategoryId.Value,
                    CategoryName = group.Key.CategoryName,
                    CategorySubId = group.Key.CategorySubId.Value,
                    CategorySubName = group.Key.CategorySubName,
                    BrandId = group.Key.BrandId.Value,
                    BrandName = group.Key.BrandName,
                    ModelId = group.Key.ModelId.Value,
                    ModelName = group.Key.ModelName,
                    ProductCount = group.Count()
                })
                .Distinct()
                .GroupBy(dto => new { dto.CategoryId, dto.CategoryName, dto.CategorySubName })
                .Select(group => new CategoryProductCountsDto
                {
                    CategoryId = group.Key.CategoryId,
                    CategoryName = group.Key.CategoryName,
                    CategorySubName = group.Key.CategorySubName,
                    ProductCount = group.Sum(dto => dto.ProductCount),
                    BrandId = group.Select(dto => dto.BrandId).FirstOrDefault(),
                    BrandName = group.Select(dto => dto.BrandName).FirstOrDefault(),
                    ModelId = group.Select(dto => dto.ModelId).FirstOrDefault(),
                    ModelName = group.Select(dto => dto.ModelName).FirstOrDefault()
                })
                .ToList();

            return Result<List<CategoryProductCountsDto>>.Success(productCounts);
        }
    }
}
