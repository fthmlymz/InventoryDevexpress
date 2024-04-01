using Domain.Common;

namespace Application.Features.Models.Commands.CreateModel
{
    public class CreatedModelDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
        public int? BrandId { get; set; }
    }
}
