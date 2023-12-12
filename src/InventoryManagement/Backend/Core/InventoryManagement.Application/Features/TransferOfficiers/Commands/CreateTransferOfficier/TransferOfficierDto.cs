using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.TransferOfficiers.Commands.CreateTransferOfficier
{
    public class TransferOfficierDto: BaseAuditableEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
