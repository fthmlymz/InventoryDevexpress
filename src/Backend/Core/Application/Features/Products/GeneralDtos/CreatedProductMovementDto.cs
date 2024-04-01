using Domain.Common;

namespace Application.Features.Products.GeneralDtos
{
    public class CreatedProductMovementDto : BaseAuditableEntity
    {
        public int? ProductId { get; set; }
        public string? Description { get; set; }
        public DateTime? MovementDate { get; set; }
    }
}
