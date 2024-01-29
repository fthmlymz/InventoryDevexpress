using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.TransferOfficiers.Commands.CreateTransferOfficier
{
    public class TransferOfficierCreatedEvent : BaseEvent
    {
        public TransferOfficier TransferOfficier { get; }
        public TransferOfficierCreatedEvent(TransferOfficier transferOfficier)
        {
            TransferOfficier = transferOfficier;
        }
    }
}
