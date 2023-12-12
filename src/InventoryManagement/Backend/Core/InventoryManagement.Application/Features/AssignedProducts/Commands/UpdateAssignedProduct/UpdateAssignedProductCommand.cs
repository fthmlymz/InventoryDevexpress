using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.AssignedProducts.Commands.UpdateAssignedProduct
{
    public sealed record UpdateAssignedProductCommand : IRequest<Result<AssignedProduct>>
    {
        public int? Id { get; set; }
        public string? AssignedUserName { get; set; }
        public string? AssignedUserId { get; set; }
        public string? AssignedUserPhoto { get; set; }
        public string? Email { get; set; }
        public string? ProductName { get; set; }
        public string? FullName { get; set; }
        public string? Barcode { get; set; }
        public int? ProductId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
