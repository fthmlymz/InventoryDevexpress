using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Products.Commands.GetStoreProduct
{
    public class UpdateGetStoreProductCommand : IRequest<Result<Product>>
    {
        public int Id { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
