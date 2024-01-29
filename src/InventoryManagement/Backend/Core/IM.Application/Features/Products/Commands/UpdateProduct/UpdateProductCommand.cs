using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Products.Commands.UpdateProduct
{
    public sealed record UpdateProductCommand : IRequest<Result<Product>>
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string? Name { get; set; }
        public string? Barcode { get; set; }
        public string? SerialNumber { get; set; }
        public string? Imei { get; set; }
        public string? Mac { get; set; }
        public string? DataClass { get; set; }
        public string? Status { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public int? CategoryId { get; set; }
        public int? CategorySubId { get; set; }
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }


        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public DateTime? ProductDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
}