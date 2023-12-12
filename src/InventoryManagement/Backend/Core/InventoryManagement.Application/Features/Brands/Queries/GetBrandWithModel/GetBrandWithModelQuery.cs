using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Brands.Queries.GetBrandWithModel
{
    public sealed record GetBrandWithModelQuery : IRequest<Result<GetBrandWithModelDto>>
    {
        public int CompanyId { get; set; }
        public GetBrandWithModelQuery() { }
        public GetBrandWithModelQuery(int companyId)
        {
            CompanyId = companyId;
        }
    }

    internal class GetBrandWithModelQueryHandler : IRequestHandler<GetBrandWithModelQuery, Result<GetBrandWithModelDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetBrandWithModelQueryHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public GetBrandWithModelQueryHandler(IUnitOfWork unitOfWork, ILogger<GetBrandWithModelQueryHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<GetBrandWithModelDto>> Handle(GetBrandWithModelQuery request, CancellationToken cancellationToken)
        {
            // Easycaching
            var cacheKey = $"BrandWithModel_{request.CompanyId}";
            var cachedResult = await _easyCacheService.GetAsync<GetBrandWithModelDto>(cacheKey);
            if (cachedResult != null)
            {
                _logger.LogInformation($"Retrieved brand with model from cache: {request.CompanyId}");
                var createdDto = cachedResult.Adapt<GetBrandWithModelDto>();
                return await Result<GetBrandWithModelDto>.SuccessAsync(createdDto);
            }


            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(request.CompanyId);
            if (brand == null)
            {
                _logger.LogInformation($"Requested brand not found: {request.CompanyId}");
                throw new NotFoundExceptionCustom(nameof(Brand));
            }


            //Brand in model
            var brandModels = await _unitOfWork.Repository<Model>()
                .Entities.Where(x => x.BrandId == request.CompanyId)
                .ToListAsync(cancellationToken);


            var dto = new GetBrandWithModelDto
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedBy = brand.CreatedBy,
                CreatedDate = brand.CreatedDate,
                UpdatedBy = brand.UpdatedBy,
                UpdatedDate = brand.UpdatedDate,
                CompanyId = brand.CompanyId,
                BrandModels = brandModels.Select(c => new GetBrandWithModelDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CreatedBy = c.CreatedBy,
                    CreatedDate = c.CreatedDate,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedDate = c.UpdatedDate
                }).ToList()
            };

            //Sonucu ön belleğe al
            await _easyCacheService.SetAsync(cacheKey, dto);

            return Result<GetBrandWithModelDto>.Success(dto);
        }
    }


}
