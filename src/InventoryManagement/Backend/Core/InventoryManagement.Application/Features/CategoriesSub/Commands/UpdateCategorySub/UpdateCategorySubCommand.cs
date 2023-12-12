using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.CategoriesSub.Commands.UpdateCategorySub
{
    public class UpdateCategorySubCommand : IRequest<Result<CategorySub>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? CategoryId { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
