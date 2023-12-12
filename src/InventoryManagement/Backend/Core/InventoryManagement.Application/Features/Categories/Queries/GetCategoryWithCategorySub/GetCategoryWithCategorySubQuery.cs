using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryWithCategorySub
{
    public sealed record GetCategoryWithCategorySubQuery : IRequest<Result<GetCategoryWithCategorySubDto>>
    {
        public int CompanyId { get; set; }
        public GetCategoryWithCategorySubQuery() { }
        public GetCategoryWithCategorySubQuery(int companyId)
        {
            CompanyId = companyId;
        }
    }

    internal class GetCategoryWithCategorySubQueryHandler : IRequestHandler<GetCategoryWithCategorySubQuery, Result<GetCategoryWithCategorySubDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCategoryWithCategorySubQueryHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public GetCategoryWithCategorySubQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCategoryWithCategorySubQueryHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<GetCategoryWithCategorySubDto>> Handle(GetCategoryWithCategorySubQuery request, CancellationToken cancellationToken)
        {
            // Easycaching
            var cacheKey = $"CategoryWithCategorySub_{request.CompanyId}";
            var cachedResult = await _easyCacheService.GetAsync<GetCategoryWithCategorySubDto>(cacheKey);
            if (cachedResult != null)
            {
                _logger.LogInformation($"Retrieved category with categorySub from cache: {request.CompanyId}");
                var createdDto = cachedResult.Adapt<GetCategoryWithCategorySubDto>();
                return await Result<GetCategoryWithCategorySubDto>.SuccessAsync(createdDto);
            }


            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.CompanyId);
            if (category == null)
            {
                _logger.LogInformation($"Requested category not found: {request.CompanyId}");
                throw new NotFoundExceptionCustom(nameof(Category));
            }

            //Category in category sub
            var categorySubs = await _unitOfWork.Repository<CategorySub>()
                .Entities.Where(x => x.CategoryId == request.CompanyId)
                .ToListAsync(cancellationToken);

            var dto = new GetCategoryWithCategorySubDto
            {
                Id = category.Id,
                Name = category.Name,
                CreatedBy = category.CreatedBy,
                CreatedDate = category.CreatedDate,
                UpdatedBy = category.UpdatedBy,
                UpdatedDate = category.UpdatedDate,
                CompanyId = category.CompanyId,

                CategorySubs = categorySubs.Select(c => new GetCategoryWithCategorySubDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CreatedBy = c.CreatedBy,
                    CreatedDate = c.CreatedDate,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedDate = c.UpdatedDate
                }).ToList(),
            };

            // Sonucu önbelleğe al
            await _easyCacheService.SetAsync(cacheKey, dto);

            return Result<GetCategoryWithCategorySubDto>.Success(dto);
        }
    }
}
