using Domain.Common;

namespace SharedLibrary.DTOs
{
    public class GetAllCompanyAndTransferOfficerDto : BaseAuditableEntity
    {
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
}
