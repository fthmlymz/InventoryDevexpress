using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Models.Commands.UpdateModel
{
    public class UpdateModelCommand : IRequest<Result<Model>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? BrandId { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
