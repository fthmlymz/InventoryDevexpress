using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Frontend.DTOs.Product
{
    public class ApproveRejectDto
    {
        [Required]
        public int? ProductId { get; set; }
        [Required]
        public int? AssignedProductId { get; set; }
        [Required]
        public string? ApprovalStatus { get; set; }
    }
}
