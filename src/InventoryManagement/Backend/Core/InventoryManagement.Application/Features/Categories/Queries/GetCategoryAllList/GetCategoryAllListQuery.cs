using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryAllList
{
    public sealed record GetCategoryAllListQuery : IRequest<Result<List<GetCategoryAllListDto>>>
    {
    }

    internal class GetCategoryAllListQueryHandler : IRequestHandler<GetCategoryAllListQuery, Result<List<GetCategoryAllListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCategoryAllListQueryHandler> _logger;
        private readonly IEasyCacheService _cacheService;

        public GetCategoryAllListQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCategoryAllListQueryHandler> logger, IEasyCacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<Result<List<GetCategoryAllListDto>>> Handle(GetCategoryAllListQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "AllCategories";
            var cachedResult = await _cacheService.GetAsync<List<GetCategoryAllListDto>>(cacheKey);
            if (cachedResult != null)
            {
                _logger.LogInformation("Retrieved all categories from cache.");
                return Result<List<GetCategoryAllListDto>>.Success(cachedResult);
            }

            var categories = await _unitOfWork.Repository<Category>()
                .Entities
                .Include(c => c.CategorySubs)
                .ToListAsync(cancellationToken);

            var dtos = categories.Select(c => new GetCategoryAllListDto
            {
                Id = c.Id,
                Name = c.Name,

                SubCategories = c.CategorySubs.Select(sub => new SubCategoryDto
                {
                    Id = sub.Id,
                    Name = sub.Name
                }).ToList()
            }).ToList();

            await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(60));

            return Result<List<GetCategoryAllListDto>>.Success(dtos);
        }
    }
}
