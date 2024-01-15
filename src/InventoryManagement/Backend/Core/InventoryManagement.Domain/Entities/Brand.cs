using InventoryManagement.Domain.Common;

namespace InventoryManagement.Domain.Entities
{
    public class Brand : BaseAuditableEntity
    {
        public string? Name { get; set; }



        //#region Relationship - Affiliated with the upper class
        //public int? CompanyId { get; set; }
        //public Company ? Company { get; set; } = default!;
        //#endregion



        #region Relationship - Fetch Subclasses
        public ICollection<Model> Models { get; set; } = new List<Model>();
        #endregion
    }
}
