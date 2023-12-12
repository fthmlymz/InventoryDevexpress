namespace InventoryManagement.Frontend.Models
{
    public class CategoryAllListModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public List<SubCategoryModel> SubCategories { get; set; } = new List<SubCategoryModel>();
    }
    
    public class SubCategoryModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
