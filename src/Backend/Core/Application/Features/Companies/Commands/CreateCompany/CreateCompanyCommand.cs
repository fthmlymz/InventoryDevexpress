using MediatR;
using Shared;

namespace Application.Features.Companies.Commands.CreateCompany
{
    public sealed record CreateCompanyCommand(string? Name, string? Description, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedCompanyDto>>;
}
