using Domain.Common;

namespace Domain.Entities
{
    public class AssignedProduct : BaseAuditableEntity
    {
        public string? AssignedUserName { get; set; }
        public string? AssignedUserId { get; set; }
        public string? AssignedUserPhoto { get; set; }
        public string? FullName { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? Company { get; set; }
        public string? PhysicalDeliveryOfficeName { get; set; }
        public string? Title { get; set; }
        public string? Manager { get; set; }
        public string? Department { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
