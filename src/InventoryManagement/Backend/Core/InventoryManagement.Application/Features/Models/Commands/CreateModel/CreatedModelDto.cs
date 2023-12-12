using InventoryManagement.Domain.Common;

namespace InventoryManagement.Application.Features.Models.Commands.CreateModel
{
    public class CreatedModelDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
        public int BrandId { get; set; }
    }
}
