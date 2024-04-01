using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.Reports.Queries.GeneralReport
{
    public sealed record CombinedProductCountsQuery() : IRequest<Result<List<CombinedProductCountsDto>>>;

    internal class CombinedProductCountsQueryHandler : IRequestHandler<CombinedProductCountsQuery, Result<List<CombinedProductCountsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CombinedProductCountsQueryHandler> _logger;

        public CombinedProductCountsQueryHandler(IUnitOfWork unitOfWork, ILogger<CombinedProductCountsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<List<CombinedProductCountsDto>>> Handle(CombinedProductCountsQuery request, CancellationToken cancellationToken)
        {
            var combinedProductCounts = new CombinedProductCountsDto();

            var productCountsQuery = _unitOfWork.Repository<Product>()
                .Entities
                .GroupBy(product => new
                {
                    product.CompanyId,
                    CompanyName = product.Company.Name,
                    product.CategorySubId,
                    CategorySubName = product.CategorySub.Name
                })
                .Select(group => new
                {
                    group.Key.CompanyId,
                    group.Key.CompanyName,
                    CategorySubId = group.Key.CategorySubId.Value,
                    group.Key.CategorySubName,
                    ProductCount = group.Count()
                })
                .ToList();
            combinedProductCounts.CompanyProductReport = productCountsQuery
                .Select(dto => new CompanyProductCountsDto
                {
                    CompanyId = dto.CompanyId,
                    CompanyName = dto.CompanyName,
                    CategorySubId = dto.CategorySubId,
                    CategorySubName = dto.CategorySubName,
                    ProductCount = dto.ProductCount
                })
                .ToList();
            combinedProductCounts.AllProductReport = productCountsQuery
                .GroupBy(dto => new { dto.CategorySubId, dto.CategorySubName })
                .Select(group => new ProductCountsAllDto
                {
                    CategorySubId = group.Key.CategorySubId,
                    CategorySubName = group.Key.CategorySubName,
                    ProductCount = group.Sum(dto => dto.ProductCount)
                })
                .ToList();
            return await Result<List<CombinedProductCountsDto>>.SuccessAsync(new List<CombinedProductCountsDto> { combinedProductCounts });
        }
    }
}
