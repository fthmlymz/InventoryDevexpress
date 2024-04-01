using Domain.Common;
using Domain.Entities;

namespace Application.Features.Models.Commands.UpdateModel
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
