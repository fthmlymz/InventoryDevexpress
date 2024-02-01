using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Frontend.DTOs.Brand
{
    public class BrandDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Marka adı gereklidir.")]
        public string ? Name { get; set; }
        public string ? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<ModelDto>? Models { get; set; } = new List<ModelDto>();
    }

    public class ModelDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Model alanı gereklidir.")]
        public string? Name { get; set; }
        public int BrandId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
