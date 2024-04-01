using InventoryManagement.Domain.Common;

namespace InventoryManagement.Frontend.DTOs.Reports
{
    public class GeneralReportDto : BaseAuditableEntity
    {
        public ICollection<CompanyProductReportItemDto>? CompanyProductReport { get; set; }
        public ICollection<AllProductReportItemDto>? AllProductReport { get; set; }
    }
    public class CompanyProductReportItemDto
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int CategorySubId { get; set; }
        public string? CategorySubName { get; set; }
        public int ProductCount { get; set; }
    }
    public class AllProductReportItemDto
    {
        public int CategorySubId { get; set; }
        public string? CategorySubName { get; set; }
        public int ProductCount { get; set; }
    }
}
