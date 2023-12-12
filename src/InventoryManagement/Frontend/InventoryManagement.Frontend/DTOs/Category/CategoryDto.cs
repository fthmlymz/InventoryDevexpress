namespace InventoryManagement.Frontend.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        //public int CompanyId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<CategorySubModel>? CategorySubs { get; set; }
    }

    public class CategorySubModel
    {
        public int Id { get; set; }
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