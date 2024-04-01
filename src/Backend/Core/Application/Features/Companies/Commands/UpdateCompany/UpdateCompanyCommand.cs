using Domain.Entities;
using MediatR;
using Shared;

namespace Application.Features.Companies.Commands.UpdateCompany
{
    public sealed record UpdateCompanyCommand : IRequest<Result<Company>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
