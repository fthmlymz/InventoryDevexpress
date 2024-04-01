using InventoryManagement.Domain.Common;
using MediatR;

namespace InventoryManagement.Application.Features.Models.Commands.CreateModel
{
    public class CreateModelEvent : BaseEvent, INotification
    {
        public Domain.Entities.Model Model { get; }
        public CreateModelEvent(Domain.Entities.Model model)
        {
            Model = model;
        }
    }
}
