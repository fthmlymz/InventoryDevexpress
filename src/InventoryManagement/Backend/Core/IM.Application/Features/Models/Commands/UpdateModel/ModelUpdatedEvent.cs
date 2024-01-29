using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Models.Commands.UpdateModel
{
    public class ModelUpdatedEvent : BaseEvent
    {
        public Model Model { get; }
        public ModelUpdatedEvent(Model model)
        {
            Model = model;
        }
    }
}
