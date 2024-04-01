using Domain.Entities;
using MediatR;
using Shared;

namespace Application.Features.Brands.Commands.UpdateBrand
{
    public sealed record UpdateBrandCommand : IRequest<Result<Brand>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
