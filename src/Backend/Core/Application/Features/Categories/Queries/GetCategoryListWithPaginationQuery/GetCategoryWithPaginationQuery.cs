using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.Categories.Queries.GetCategoryListWithPaginationQuery
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
            //_easyCacheService = easyCacheService;
        }

        public async Task<PaginatedResult<GetCategoryWithPaginationDto>> Handle(GetCategoryWithPaginationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //var cacheKey = $"GetCategoryWithPagination_{request.PageNumber}_{request.PageSize}";
                //var cachedResult = await _easyCacheService.GetAsync<PaginatedResult<GetCategoryWithPaginationDto>>(cacheKey);
                //if (cachedResult != null)
                // {
                //     _logger.LogInformation("Retrieved categories from cache.");
                //     return cachedResult;
                // }

                var query = _unitOfWork.Repository<Category>().Entities.Include(c => c.CategorySubs).OrderBy(c => c.Name);

                var totalCount = await query.CountAsync(cancellationToken);

                var categories = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var dtos = categories.Select(c => new GetCategoryWithPaginationDto
                {
                    Id = c.Id,
                    Name = c.Name,

                    CategorySubs = c.CategorySubs.Select(sub => new CategorySubModel
                    {
                        Id = sub.Id,
                        Name = sub.Name,
                        CategoryId = sub.CategoryId,
                        CreatedBy = sub.CreatedBy,
                        CreatedUserId = sub.CreatedUserId,
                        CreatedDate = Convert.ToDateTime(sub.CreatedDate),
                        UpdatedBy = sub.UpdatedBy,
                        UpdatedUserId = sub.UpdatedUserId,
                        UpdatedDate = Convert.ToDateTime(sub.UpdatedDate),
                    }).ToList(),
                }).ToList();

                //var expirationTime = TimeSpan.Parse(_configuration["RedisConnectionSettings:DefaultExpiration"]);

                var result = new PaginatedResult<GetCategoryWithPaginationDto>(true, dtos, count: totalCount, pageNumber: request.PageNumber, pageSize: request.PageSize);

                // await _easyCacheService.SetAsync(cacheKey, result, expirationTime);


                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing GetCategoryWithPaginationQuery.");
                throw;
            }
        }
    }
}
