using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.AssignedProducts.Dtos
{
    public class AssignedProductDto : BaseAuditableEntity
    {
        public string? AssignedUserName { get; set; }
        public string? AssignedUserId { get; set; }
        public string? AssignedUserPhoto { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Barcode { get; set; }
        public string? ProductName { get; set; }

        public int ProductId { get; set; }
    }
}
