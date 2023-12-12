using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Companies.Queries.GetCompanyAllList
{
    public sealed record GetCompanyAllListQuery : IRequest<Result<List<GetCompanyAllListDto>>>
    {
    }

    internal class GetCompanyAllListQueryHandler : IRequestHandler<GetCompanyAllListQuery, Result<List<GetCompanyAllListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCompanyAllListQueryHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public GetCompanyAllListQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCompanyAllListQueryHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<List<GetCompanyAllListDto>>> Handle(GetCompanyAllListQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "AllCompanies";
            var cachedResult = await _easyCacheService.GetAsync<List<GetCompanyAllListDto>>(cacheKey);
            if (cachedResult != null)
            {
                _logger.LogInformation("Retrieved companies from cache");
                return Result<List<GetCompanyAllListDto>>.Success(cachedResult);
            }

            var companies = await _unitOfWork.Repository<Company>().Entities.ToListAsync(cancellationToken);

            var dtos = companies.Select(c => new GetCompanyAllListDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList();

            await _easyCacheService.SetAsync(cacheKey, dtos);

            return Result<List<GetCompanyAllListDto>>.Success(dtos);
        }
    }
}
