using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Brands.Commands.DeleteBrand
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
