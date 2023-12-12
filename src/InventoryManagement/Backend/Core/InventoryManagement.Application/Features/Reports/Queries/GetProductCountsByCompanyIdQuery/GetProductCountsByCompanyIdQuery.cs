using MediatR;

namespace InventoryManagement.Application.Features.Reports.Queries.GetProductCountsByCompanyIdQuery
{
    //public class GetProductCountsByCompanyIdQuery : IRequest<ProductCountsDto>
    //{
    //    public int CompanyId { get; set; }

    //    public GetProductCountsByCompanyIdQuery(int companyId)
    //    {
    //        CompanyId = companyId;
    //    }
    //}

    //public class GetProductCountsByCompanyIdQueryHandler : IRequestHandler<GetProductCountsByCompanyIdQuery, ProductCountsDto>
    //{
    //    private readonly ApplicationDbContext _context;

    //    public GetProductCountsByCompanyIdQueryHandler(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<ProductCountsDto> Handle(GetProductCountsByCompanyIdQuery request, CancellationToken cancellationToken)
    //    {
    //        var productCounts = new ProductCountsDto
    //        {
    //            CompanyId = request.CompanyId,
    //            ProductCount = await _context.Products
    //                .Where(p => p.CompanyId == request.CompanyId)
    //                .CountAsync(),

    //            BrandCount = await _context.Brands
    //                .Where(b => b.CompanyId == request.CompanyId)
    //                .CountAsync(),

    //            ModelCount = await _context.Models
    //                .Where(m => m.Brand.CompanyId == request.CompanyId)
    //                .CountAsync(),

    //            CategoryCount = await _context.Categories
    //                .Where(c => c.CompanyId == request.CompanyId)
    //                .CountAsync(),

    //            CategorySubCount = await _context.CategorySubs
    //                .Where(cs => cs.Category.CompanyId == request.CompanyId)
    //                .CountAsync()
    //        };

    //        return productCounts;
    //    }
    //}
}
