using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Frontend.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Ürün adı zorunludur")]
        public string? Name { get; set; }
        
        
        [Required(ErrorMessage = "Barkod alanı zorunludur")]
        public int? Barcode { get; set; }


        public string? SerialNumber { get; set; }
        public string? Imei { get; set; }
        public string? Mac { get; set; }
        public string? DataClass { get; set; }
        public string? Status { get; set; }
        public DateTime? PurchaseDate { get; set; }

        [Required(ErrorMessage = "Kategori alanı zorunludur")]
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        
        [Required(ErrorMessage = "Alt Kategori alanı zorunludur")]
        public int? CategorySubId { get; set; }
        public string? CategorySubName { get; set; }

        [Required(ErrorMessage = "Marka alanı zorunludur")]
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }

        [Required(ErrorMessage = "Model alanı zorunludur")]
        public int? ModelId { get; set; }
        public string? ModelName { get; set; }

        public DateTime? ProductDate { get; set; }
        public DateTime? InvoiceDate { get; set; }



        #region Relationship - Affiliated with the upper class
        [Required(ErrorMessage = "Şirket seçimi zorunludur")]
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        #endregion


        #region Product Assigned
        public string? AssignedUserName { get; set; }
        public string? AssignedUserId { get; set; }
        public string? FullName { get; set; }
        public string? ApprovalStatus { get; set; }
        public int? ProductId { get; set; }
        #endregion



        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
