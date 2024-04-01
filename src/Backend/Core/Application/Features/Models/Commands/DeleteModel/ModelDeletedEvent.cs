using Domain.Common;
using Domain.Entities;

namespace Application.Features.Models.Commands.DeleteModel
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
