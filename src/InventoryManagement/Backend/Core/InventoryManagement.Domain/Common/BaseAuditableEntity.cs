using InventoryManagement.Domain.Common.Interfaces;

namespace InventoryManagement.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
    {
        public int  Id { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public string? CreatedUserId { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }


        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
    }
}
