using Domain.Entities;
using MediatR;
using Shared;

namespace Application.Features.Categories.Commands.UpdateCategory
{
    public sealed record UpdateCategoryCommand : IRequest<Result<Category>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
