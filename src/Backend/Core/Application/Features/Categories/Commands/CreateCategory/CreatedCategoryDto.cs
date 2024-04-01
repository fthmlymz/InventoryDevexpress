using Domain.Common;

namespace Application.Features.Categories.Commands.CreateCategory
{
    public class CreatedCategoryDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
    }
}
