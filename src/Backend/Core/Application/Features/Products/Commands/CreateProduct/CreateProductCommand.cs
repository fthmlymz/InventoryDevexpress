using MediatR;
using Shared;

namespace Application.Features.Products.Commands.CreateProduct
{
    public sealed record CreateProductCommand(string? Name, int? Barcode, int CompanyId, string? SerialNumber, string? DataClass,
                                              int? CategoryId, int? CategorySubId, int? BrandId, int? ModelId,
                                              DateTime? PurchaseDate, string? Imei, string? Mac,
                                              string? CreatedBy, string? CreatedUserId, string? Status) : IRequest<Result<CreatedProductDto>>;
}
