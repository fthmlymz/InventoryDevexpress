using InventoryManagement.Domain.Common;

namespace InventoryManagement.Frontend.Models
{
    public class GetByIdProductAndDetailsDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
        public string? Barcode { get; set; }
        public string? SerialNumber { get; set; }
        public string? Imei { get; set; }
        public string? Mac { get; set; }
        public string? DataClass { get; set; }
        public string? Status { get; set; }
        public DateTime? ProductDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int? CategoryId { get; set; }
        public int? CategorySubId { get; set; }
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }



        public ICollection<ProductMovementQueryDto>? ProductMovements { get; set; }
        public ICollection<AssignedProductsQueryDto>? AssignedProducts { get; set; }
        public ICollection<ProductTransfersQuerDto>? ProductTransfers { get; set; }
    }




    public class ProductMovementQueryDto : BaseAuditableEntity
    {
        public int? ProductId { get; set; }
        public DateTime? MovementDate { get; set; }
        public string? Description { get; set; }
    }

    public class AssignedProductsQueryDto : BaseAuditableEntity
    {
        public string? AssignedUserName { get; set; }
        public string? AssignedUserId { get; set; }
        public string? AssignedUserPhoto { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Barcode { get; set; }
        public string? ProductName { get; set; }
        public int? ProductId { get; set; }
        public string? ApprovalStatus { get; set; }

        public ICollection<AssignedProductMovementQueryDto>? AssignedProductMovements { get; set; }
    }

    public class AssignedProductMovementQueryDto : BaseAuditableEntity
    {
        public string? AssignedUserName { get; set; }
        public string? AssignedUserId { get; set; }
        public int? AssignedProductId { get; set; }
        public DateTime? MovementDate { get; set; }
        public string? MovementDescription { get; set; }
    }

    public class ProductTransfersQuerDto
    {
        public string? SenderUserName { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderCompanyName { get; set; }

        public int? RecipientCompanyId { get; set; }
        public string? RecipientUserName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientCompanyName { get; set; }
    }
}
