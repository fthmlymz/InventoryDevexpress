using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Frontend.DTOs.Company
{
    public class CompanyDto
    {
        [Required(ErrorMessage = "Şirket Id alanı zorunludur")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Şirket adı zorunludur")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
