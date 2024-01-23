using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Brands.Queries.GetBrandListWithPaginationQuery
{
    public sealed record GetBrandWithPaginationQuery : IRequest<PaginatedResult<GetBrandWithPaginationDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetBrandWithPaginationQuery() { }
        public GetBrandWithPaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    internal class GetBrandQueryHandler : IRequestHandler<GetBrandWithPaginationQuery, PaginatedResult<GetBrandWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetBrandQueryHandler> _logger;

        public GetBrandQueryHandler(IUnitOfWork unitOfWork, ILogger<GetBrandQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaginatedResult<GetBrandWithPaginationDto>> Handle(GetBrandWithPaginationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _unitOfWork.Repository<Brand>().Entities.Include(c => c.Models).OrderBy(c => c.Name);
                var totalCount = await query.CountAsync(cancellationToken);

                var brands = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var dtos = brands.Select(c => new GetBrandWithPaginationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Models = c.Models.Select(sub => new ModelDto
                    {
                        Id = sub.Id,
                        Name = sub.Name,
                        BrandId = sub.BrandId.Value,
                        CreatedBy = sub.CreatedBy,
                        CreatedDate = sub.CreatedDate,
                        CreatedUserId = sub.CreatedUserId,
                        UpdatedBy = sub.UpdatedBy,
                        UpdatedDate = sub.UpdatedDate,
                        UpdatedUserId = sub.UpdatedUserId
                    }).ToList(),
                }).ToList();

                var result = new PaginatedResult<GetBrandWithPaginationDto>(true, dtos, count: totalCount, pageNumber: request.PageNumber, pageSize: request.PageSize);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing GetCategoryWithPaginationQuery.");
                throw;
            }
            /*var query = _unitOfWork.Repository<Brand>().Entities.OrderBy(c => c.Name);
            var totalCount = await query.CountAsync(cancellationToken);

            var brands = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectToType<GetBrandWithPaginationDto>()
                .ToListAsync(cancellationToken);

            return new PaginatedResult<GetBrandWithPaginationDto>(true, brands, count: totalCount, pageNumber: request.PageNumber, pageSize: request.PageSize);*/
        }
    }
}
