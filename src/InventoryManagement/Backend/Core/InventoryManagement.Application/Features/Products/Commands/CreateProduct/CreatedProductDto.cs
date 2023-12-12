using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Products.Commands.CreateProduct
{
    public class CreatedProductDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
        public int? CompanyId { get; set; }
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


        public DateTime? ProductDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
}
