using InventoryManagement.Domain.Common;

namespace InventoryManagement.Frontend.DTOs.Company
{
    public class CompanyAndTransferOfficerModel : BaseAuditableEntity
    {
        public int? CompanyId { get; set; }
        public string? Name { get; set; }
        public ICollection<TransferOfficerQueryDto>? TransferOfficers { get; set; }
    }
    public class TransferOfficerQueryDto : BaseAuditableEntity
    {
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int? CompanyId { get; set; }
    }
}
