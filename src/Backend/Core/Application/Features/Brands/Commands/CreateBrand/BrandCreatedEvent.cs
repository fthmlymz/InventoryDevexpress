using Domain.Common;

namespace Application.Features.Brands.Commands.CreateBrand
{
    public class BrandCreatedEvent : BaseEvent
    {
        public Domain.Entities.Brand Brand { get; }
        public BrandCreatedEvent(Domain.Entities.Brand brand)
        {
            Brand = brand;
        }
    }
}
