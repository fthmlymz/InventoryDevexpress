using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Frontend.DTOs.TransferOfficier
{
    public class TransferOfficierDto
    {
        public int Id { get; set; }
        public string ? FullName { get; set; }
        public string ? UserName { get; set; }
        public string ? Email { get; set; }
        [Required(ErrorMessage = "Şirket adı zorunludur.")]
        public int CompanyId { get; set; }
        public string ? CreatedBy { get; set; }
        public string ? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ? UpdatedBy { get; set; }
        public string ? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
