using Domain.Entities;
using MediatR;
using Shared;

namespace Application.Features.Products.Commands.GetStoreProduct
{
    public class UpdateGetStoreProductCommand : IRequest<Result<Product>>
    {
        public int Id { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
