using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Companies.Queries.GetCompanyWithPagination
{
    public class GetCompanyListWithPaginationDto : BaseAuditableEntity
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public int Id { get; set; }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}
