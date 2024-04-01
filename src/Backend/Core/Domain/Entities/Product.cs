using Domain.Common;

namespace Domain.Entities
{
    public class Product : BaseAuditableEntity
    {
        public string? Name { get; set; }
        public int Barcode { get; set; }
        public string? SerialNumber { get; set; }
        public string? Imei { get; set; }
        public string? Mac { get; set; }
        public string? DataClass { get; set; }
        public string? Status { get; set; }
        public string? WorkflowId { get; set; }
        public DateTime? PurchaseDate { get; set; }


        public DateTime? ProductDate { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? CategorySubId { get; set; }
        public CategorySub? CategorySub { get; set; }
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public int? ModelId { get; set; }
        public Model? Model { get; set; }



        #region Relationship - Affiliated with the upper class
        public int CompanyId { get; set; }
        public Company Company { get; set; } = default!;
        #endregion



        #region Relationship - 
        public ICollection<AssignedProduct>? AssignedProducts { get; set; }
        public ICollection<ProductMovement>? ProductMovements { get; set; }
        #endregion
    }
}
