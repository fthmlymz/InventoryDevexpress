using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryListWithPaginationQuery
{
    public sealed record GetCategoryWithPaginationQuery : IRequest<PaginatedResult<GetCategoryWithPaginationDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public GetCategoryWithPaginationQuery() { }
        public GetCategoryWithPaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
    internal class GetCategoryQueryHandler : IRequestHandler<GetCategoryWithPaginationQuery, PaginatedResult<GetCategoryWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IEasyCacheService _easyCacheService;
        private readonly ILogger<GetCategoryQueryHandler> _logger;

        public GetCategoryQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCategoryQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            // _easyCacheService = easyCacheService;
        }

        public async Task<PaginatedResult<GetCategoryWithPaginationDto>> Handle(GetCategoryWithPaginationQuery request, CancellationToken cancellationToken)
        {
            //var cacheKey = $"Categories_{request.PageNumber}:{request.PageSize}";
            //var cachedResult = await _easyCacheService.GetAsync<GetCategoryWithPaginationDto>(cacheKey);
            //if (cachedResult != null)
            //{
            //    _logger.LogInformation($"Retrieved company with categories from cache: {request.PageNumber} - {request.PageSize}");
            //    var createdDto = cachedResult.Adapt<GetCategoryWithPaginationDto>();
            //    return await PaginatedResult<GetCategoryWithPaginationDto>.SuccessAsync(createdDto);
            //}

            var query = _unitOfWork.Repository<Category>().Entities.OrderBy(c => c.Name);

            var totalCount = await query.CountAsync(cancellationToken);

            var categories = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectToType<GetCategoryWithPaginationDto>()
                .ToListAsync(cancellationToken);

            return new PaginatedResult<GetCategoryWithPaginationDto>(true, categories, count: totalCount, pageNumber: request.PageNumber, pageSize: request.PageSize);
        }
    }
}
