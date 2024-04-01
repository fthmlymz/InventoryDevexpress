using Domain.Common;

namespace Domain.Entities
{
    public class Category : BaseAuditableEntity
    {
        public string? Name { get; set; }




        //#region Relationship - Affiliated with the upper class
        //public int? CompanyId { get; set; }
        //public Company? Company { get; set; }
        //#endregion




        #region Relationship - Fetch Subclasses
        public ICollection<CategorySub> CategorySubs { get; set; } = new List<CategorySub>();
        #endregion
    }
}
