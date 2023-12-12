using InventoryManagement.Application.Extensions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Companies.Queries.GetCompanyWithPagination
{
    public record GetCompanyListWithPaginationQuery : IRequest<PaginatedResult<GetCompanyListWithPaginationDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetCompanyListWithPaginationQuery() { }
        public GetCompanyListWithPaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    internal class GetCompanyWithPaginationQueryHandler : IRequestHandler<GetCompanyListWithPaginationQuery, PaginatedResult<GetCompanyListWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCompanyWithPaginationQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        //Best performance, sıralama veritabanından yapıldı
        public async Task<PaginatedResult<GetCompanyListWithPaginationDto>> Handle(GetCompanyListWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Company>().Entities.OrderBy(x => x.Name);

            var paginatedQuery = query
                .Select(x => new GetCompanyListWithPaginationDto
                {
                    // İhtiyaç duyulan alanları buraya ekleyin
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                });

            return await paginatedQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}

/*Original code
public async Task<PaginatedResult<GetCompanyListWithPaginationDto>> Handle(GetCompanyListWithPaginationQuery request, CancellationToken cancellationToken)
{
    return await _unitOfWork.Repository<Company>().Entities
        .OrderBy(x => x.Name)
        .ProjectToType<GetCompanyListWithPaginationDto>()
        .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
}*/