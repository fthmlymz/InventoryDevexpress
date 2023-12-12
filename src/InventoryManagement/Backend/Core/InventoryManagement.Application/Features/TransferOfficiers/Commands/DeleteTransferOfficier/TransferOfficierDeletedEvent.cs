using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.TransferOfficiers.Commands.DeleteTransferOfficier
{
    public class TransferOfficierDeletedEvent : BaseEvent
    {
        public TransferOfficier TransferOfficier { get; }

        public TransferOfficierDeletedEvent(TransferOfficier transferOfficier)
        {
            TransferOfficier = transferOfficier;
        }
    }
}
