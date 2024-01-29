using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Frontend.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Kategori adı gereklidir")]
        public string? Name { get; set; }
        //public int CompanyId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<CategorySubDto>? CategorySubs { get; set; } = new List<CategorySubDto>();
    }

    public class CategorySubDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Alt kategori adı gereklidir")]
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}