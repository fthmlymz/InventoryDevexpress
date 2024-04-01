using Domain.Common;

namespace Application.Features.Brands.Commands.DeleteBrand
{
    public class BrandDeletedEvent : BaseEvent
    {
        public Domain.Entities.Brand Brand { get; set; }
        public BrandDeletedEvent(Domain.Entities.Brand brand)
        {
            Brand = brand;
        }
    }
}
