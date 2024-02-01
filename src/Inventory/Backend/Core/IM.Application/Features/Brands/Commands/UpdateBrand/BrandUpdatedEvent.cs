using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Brands.Commands.UpdateBrand
{
    public class BrandUpdatedEvent : BaseEvent
    {
        public Domain.Entities.Brand Brand { get; set; }
        public BrandUpdatedEvent(Domain.Entities.Brand brand)
        {
            Brand = brand;
        }
    }
}
