namespace InventoryManagement.Application.Features.Reports.Queries.GetProductCountsQuery
{
    public class CategoryProductCountsDto
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int CategorySubId { get; set; }
        public string? CategorySubName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public int ModelId { get; set; }
        public string? ModelName { get; set; }
        public int ProductCount { get; set; }
    }
}
