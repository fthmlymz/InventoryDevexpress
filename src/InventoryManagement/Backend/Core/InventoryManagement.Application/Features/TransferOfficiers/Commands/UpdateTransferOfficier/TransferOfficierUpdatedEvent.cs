using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.TransferOfficiers.Commands.UpdateTransferOfficier
{
    public class TransferOfficierUpdatedEvent : BaseEvent
    {
        public TransferOfficier TransferOfficier { get; }
        public TransferOfficierUpdatedEvent(TransferOfficier transferOfficier)
        {
            TransferOfficier = transferOfficier;
        }
    }
}
