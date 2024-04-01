using Domain.Common;

namespace Application.Features.Brands.Commands.CreateBrand
{
    public class CreatedBrandDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
    }
}
