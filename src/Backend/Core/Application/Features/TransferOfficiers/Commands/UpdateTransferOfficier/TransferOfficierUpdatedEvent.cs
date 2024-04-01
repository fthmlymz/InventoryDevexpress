using Domain.Common;
using Domain.Entities;

namespace Application.Features.TransferOfficiers.Commands.UpdateTransferOfficier
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
