using Domain.Common;

namespace Application.Features.CategoriesSub.Commands.CreateCategorySub
{
    public class CreatedCategorySubDto : BaseAuditableEntity
    {
        public string? Name { get; set; }
        public int CategoryId { get; set; }
    }
}
