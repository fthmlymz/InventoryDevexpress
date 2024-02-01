using InventoryManagement.Domain.Common;

namespace InventoryManagement.Domain.Entities
{
    public class AssignedProduct : BaseAuditableEntity
    {
        public string? AssignedUserName { get; set; }
        public string? AssignedUserId { get; set; }
        public string? AssignedUserPhoto { get; set; }
        public string? FullName { get; set; }
        public string? ApprovalStatus { get; set; }


        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
