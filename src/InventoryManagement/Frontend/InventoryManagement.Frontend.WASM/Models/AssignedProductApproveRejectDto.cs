namespace InventoryManagement.Frontend.Models
{
    public class AssignedProductApproveRejectDto
    {
        public int? ProductId { get; set; }
        public int? AssignedProductId { get; set; }
        public string? ApprovalStatus { get; set; }
    }
}
