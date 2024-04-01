using Domain.Common;

namespace Application.Features.TransferOfficiers.Commands.CreateTransferOfficier
{
    public class TransferOfficierDto : BaseAuditableEntity
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public int CompanyId { get; set; }
    }
}
