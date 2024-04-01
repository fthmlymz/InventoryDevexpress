using Domain.Common;

namespace Application.Features.Brands.Queries.GetBrandListWithPaginationQuery
{
    public class GetBrandWithPaginationDto : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public List<ModelDto>? Models { get; set; } = new List<ModelDto>();
    }
    public class ModelDto
    {
        public int Id { get; set; }
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
