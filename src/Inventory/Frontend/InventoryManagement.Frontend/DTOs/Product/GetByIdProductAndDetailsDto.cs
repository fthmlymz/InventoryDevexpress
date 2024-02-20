using InventoryManagement.Domain.Common;

namespace InventoryManagement.Frontend.DTOs.Product
{
    public class GetByIdProductAndDetailsDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
        public int? Barcode { get; set; }
        public string? SerialNumber { get; set; }
        public string? Imei { get; set; }
        public string? Mac { get; set; }
        public string? DataClass { get; set; }
        public string? Status { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public DateTime? ProductDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int? CategoryId { get; set; }
        public int? CategorySubId { get; set; }
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }
        public int? ModelId { get; set; }
        public string? ModelName { get; set; }



        public ICollection<ProductMovementQueryDto>? ProductMovements { get; set; }
        public ICollection<AssignedProductQueryDto>? AssignedProducts { get; set; }
        public ICollection<ProductTransferQuerDto>? ProductTransfers { get; set; }
    }




    public class ProductMovementQueryDto : BaseAuditableEntity
    {
        public int? ProductId { get; set; }
        public DateTime? MovementDate { get; set; }
        public string? Description { get; set; }
    }

    public class AssignedProductQueryDto : BaseAuditableEntity
    {
        public string? AssignedUserName { get; set; }
        public string? AssignedUserId { get; set; }
        public string? AssignedUserPhoto { get; set; }
        public string? FullName { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? Department { get; set; }
        public string? Manager { get; set; }
        public string? PhysicalDeliveryOfficeName { get; set; }
        public string? Title { get; set; }
        public int? ProductId { get; set; }



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

    public class ProductTransferQuerDto
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
