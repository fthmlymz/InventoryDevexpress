using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Features.Brands.Queries.GetBrandListWithPaginationQuery;
using InventoryManagement.Application.Features.Categories.Queries.GetCategoryListWithPaginationQuery;
using InventoryManagement.Application.Features.Companies.Queries.GetCompanyByIdWithCategory;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Companies.Queries.GetCompanyById
{
    public sealed record GetCompanyByIdWithCategoryQuery : IRequest<Result<GetCompanyByIdWithCategoryDto>>
    {
        public int Id { get; set; }
        public GetCompanyByIdWithCategoryQuery() { }
        public GetCompanyByIdWithCategoryQuery(int id)
        {
            Id = id;
        }
    }
    internal class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdWithCategoryQuery, Result<GetCompanyByIdWithCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEasyCacheService _easyCacheService;
        private readonly ILogger<GetCompanyByIdQueryHandler> _logger;

        public GetCompanyByIdQueryHandler(IUnitOfWork unitOfWork, IEasyCacheService easyCacheService, ILogger<GetCompanyByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _easyCacheService = easyCacheService;
            _logger = logger;
        }

        //Best performance
        public async Task<Result<GetCompanyByIdWithCategoryDto>> Handle(GetCompanyByIdWithCategoryQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"CompanyWithCategory_{request.Id}";
            var cachedResult = await _easyCacheService.GetAsync<GetCompanyByIdWithCategoryDto>(cacheKey);
            if (cachedResult != null)
            {
                _logger.LogInformation($"Retrieved company with categories from cache: {request.Id}");
                var createdDto = cachedResult.Adapt<GetCompanyByIdWithCategoryDto>();
                return await Result<GetCompanyByIdWithCategoryDto>.SuccessAsync(createdDto);
            }


            var company = await _unitOfWork.Repository<Company>().GetByIdAsync(request.Id);
            if (company == null)
            {
                _logger.LogInformation($"Requested company not found: {request.Id}");
                throw new NotFoundExceptionCustom(nameof(company));
            }

            var categories = await _unitOfWork.Repository<Category>()
                .Entities.Where(x => x.CompanyId == request.Id)
                .ToListAsync(cancellationToken);

            var categorySubs = await _unitOfWork.Repository<CategorySub>()
                .Entities.Where(x => categories.Select(c => c.Id).Contains(x.CategoryId))
                .ToListAsync(cancellationToken);


            var brands = await _unitOfWork.Repository<Brand>()
                .Entities.Where(x => x.CompanyId == request.Id)
                .ToListAsync(cancellationToken);
            var brandModels = await _unitOfWork.Repository<Model>()
                .Entities.Where(x => brands.Select(c => c.Id).Contains(x.BrandId))
                .ToListAsync(cancellationToken);


            var dto = new GetCompanyByIdWithCategoryDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Categories = categories.Select(c => new GetCategoryWithPaginationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    //CompanyId = c.CompanyId,
                    CreatedBy = company.CreatedBy,
                    CreatedDate = company.CreatedDate,
                    UpdatedBy = company.UpdatedBy,
                    UpdatedDate = company.UpdatedDate,
                    //CreatedUserId = company.CreatedUserId,
                    CategorySubs = categorySubs
                        .Where(cs => cs.CategoryId == c.Id)
                        .Select(categorySub => new GetCompanyAndCategoryAndCategorySubDto
                        {
                            Id = categorySub.Id,
                            Name = categorySub.Name,
                            CategoryId = categorySub.CategoryId,
                            CreatedBy = categorySub.CreatedBy,
                            CreatedDate = categorySub.CreatedDate,
                            UpdatedBy = categorySub.UpdatedBy,
                            UpdatedDate = categorySub.UpdatedDate,
                            CreatedUserId = categorySub.CreatedUserId,
                            UpdatedUserId = categorySub.UpdatedUserId
                        }).ToList()
                }).ToList(),

                Brands = brands.Select(c => new GetBrandWithPaginationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CompanyId = c.CompanyId,
                    CreatedBy = company.CreatedBy,
                    CreatedDate = company.CreatedDate,
                    UpdatedBy = company.UpdatedBy,
                    UpdatedDate = company.UpdatedDate,
                    Models = brandModels
                        .Where(cs => cs.BrandId == c.Id)
                        .Select(brandModel => new GetCompanyAndBrandAndModelDto
                        {
                            Id = brandModel.Id,
                            Name = brandModel.Name,
                            BrandId = brandModel.BrandId,
                            CreatedBy = brandModel.CreatedBy,
                            CreatedDate = brandModel.CreatedDate,
                            UpdatedBy = brandModel.UpdatedBy,
                            UpdatedDate = brandModel.UpdatedDate,
                            CreatedUserId = brandModel.CreatedUserId,
                            UpdatedUserId = brandModel.UpdatedUserId
                        }).ToList()
                }).ToList(),
            };

            await _easyCacheService.SetAsync(cacheKey, dto);

            return await Result<GetCompanyByIdWithCategoryDto>.SuccessAsync(dto);

            //Birleştirilmiş sorgu bu sorgu için daha sonra test uygulamasında hız kontrol edilecek
            /*var company = await _unitOfWork.Repository<Company>().GetByIdAsync(request.Id);
            if (company == null)
            {
                _logger.LogInformation($"Requested company not found: {request.Id}");
                throw new NotFoundExceptionCustom(nameof(company));
            }

            var categories = await _unitOfWork.Repository<Category>()
                .Entities.Where(x => x.CompanyId == request.Id)
                .ToListAsync(cancellationToken);

            var categoryIds = categories.Select(c => c.Id).ToList();

            var categorySubs = await _unitOfWork.Repository<CategorySub>()
                .Entities.Where(x => categoryIds.Contains(x.CategoryId))
                .ToListAsync(cancellationToken);

            var dto = new GetCompanyByIdWithCategoryDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Categories = categories.Select(c => new GetCategoryWithPaginationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CompanyId = c.CompanyId,
                    CreatedBy = company.CreatedBy,
                    CreatedDate = company.CreatedDate,
                    UpdatedBy = company.UpdatedBy,
                    UpdatedDate = company.UpdatedDate,
                    CategorySubs = categorySubs
                        .Where(cs => cs.CategoryId == c.Id)
                        .Select(categorySub => new GetCompanyAndCategoryAndCategorySubDto
                        {
                            Id = categorySub.Id,
                            Name = categorySub.Name,
                            CategoryId = categorySub.CategoryId,
                            CreatedBy = categorySub.CreatedBy,
                            CreatedDate = categorySub.CreatedDate,
                            UpdatedBy = categorySub.UpdatedBy,
                            UpdatedDate = categorySub.UpdatedDate,
                            CreatedUserId = categorySub.CreatedUserId,
                            UpdatedUserId = categorySub.UpdatedUserId
                        })
                        .ToList()
                }).ToList()
            };

            await _easyCacheService.SetAsync(cacheKey, dto);

            return await Result<GetCompanyByIdWithCategoryDto>.SuccessAsync(dto);*/
        }
    }
}
