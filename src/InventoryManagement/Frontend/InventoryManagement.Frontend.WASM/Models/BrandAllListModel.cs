namespace InventoryManagement.Frontend.Models
{
    public class BrandAllListModel
    {
        public int ? Id { get; set; }
        public string? Name { get; set; }
        public List<SubModel> SubModels { get; set; } = new List<SubModel>();
    }


    public class SubModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
