using InventoryManagement.Domain.Common;

namespace InventoryManagement.Domain.Entities
{
    public class TransferOfficier : BaseAuditableEntity
    {
        public string ? FullName { get; set; }
        public string ? UserName { get; set; }
        public string ? Email { get; set; }
        


        #region Relationship - Affiliated with the upper class
        public int CompanyId { get; set; }
        public Company Company { get; set; } = default!;
        #endregion
    }
}
