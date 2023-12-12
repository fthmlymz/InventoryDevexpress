namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryAllList
{
    public class GetCategoryAllListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<SubCategoryDto> SubCategories { get; set; } = new List<SubCategoryDto>();
    }
    public class SubCategoryDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
