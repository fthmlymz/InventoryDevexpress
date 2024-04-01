using Domain.Common;

namespace Domain.Entities
{
    public class ProductMovement : BaseAuditableEntity
    {


        public DateTime? MovementDate { get; set; }
        public string? Description { get; set; }


        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
