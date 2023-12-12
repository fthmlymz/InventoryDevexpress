using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Brands.Queries.GetBrandAllList
{
    public sealed record GetBrandAllListQuery : IRequest<Result<List<GetBrandAllListDto>>> { }

    internal class GetBrandAllListQueryHandler : IRequestHandler<GetBrandAllListQuery, Result<List<GetBrandAllListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEasyCacheService _cacheService;
        private readonly ILogger<GetBrandAllListQueryHandler> _logger;

        public GetBrandAllListQueryHandler(IUnitOfWork unitOfWork, IEasyCacheService cacheService, ILogger<GetBrandAllListQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<List<GetBrandAllListDto>>> Handle(GetBrandAllListQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "AllBrands";
            var cachedResult = await _cacheService.GetAsync<List<GetBrandAllListDto>>(cacheKey);
            if (cachedResult != null)
            {
                _logger.LogInformation("Retrieved all brands from cache.");
                return Result<List<GetBrandAllListDto>>.Success(cachedResult);
            }

            var brands = await _unitOfWork.Repository<Brand>()
                .Entities
                .Include(c => c.Models)
                .ToListAsync(cancellationToken);

            var dtos = brands.Select(c => new GetBrandAllListDto
            {
                Id = c.Id,
                Name = c.Name,
                SubModels = c.Models.Select(sub => new SubModelsDto
                {
                    Id = sub.Id,
                    Name = sub.Name
                }).ToList()
            }).ToList();

            await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(30));

            return Result<List<GetBrandAllListDto>>.Success(dtos);
        }
    }
}
