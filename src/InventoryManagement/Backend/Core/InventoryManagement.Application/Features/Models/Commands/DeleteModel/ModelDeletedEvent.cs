using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Models.Commands.DeleteModel
{
    public class ModelDeletedEvent : BaseEvent
    {
        public Model Model { get; }
        public ModelDeletedEvent(Model model)
        {
            Model = model;
        }
    }
}
