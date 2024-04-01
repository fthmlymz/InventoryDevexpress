using Domain.Common;

namespace Application.Features.Products.GeneralDtos
{
    public class UpdatedProductMovementDto : BaseAuditableEntity
    {
        public int? ProductId { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? MovementDate { get; set; }
    }
}
