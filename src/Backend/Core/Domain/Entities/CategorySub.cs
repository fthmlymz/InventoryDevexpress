using Domain.Common;

namespace Domain.Entities
{
    public class CategorySub : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;



        #region Relationship - Affiliated with the upper class
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        #endregion
    }
}
