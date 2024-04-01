using Domain.Common;
using Domain.Entities;

namespace Application.Features.TransferOfficiers.Commands.CreateTransferOfficier
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
