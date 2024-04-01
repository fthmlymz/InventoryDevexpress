using Domain.Common;
using Domain.Entities;

namespace Application.Features.TransferOfficiers.Commands.DeleteTransferOfficier
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
