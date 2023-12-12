namespace InventoryManagement.Application.Features.Brands.Queries.GetBrandAllList
{
    public class GetBrandAllListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<SubModelsDto> SubModels { get; set; } = new List<SubModelsDto>();
    }
    public class SubModelsDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
