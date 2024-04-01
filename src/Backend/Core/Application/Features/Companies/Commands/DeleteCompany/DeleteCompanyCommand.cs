using MediatR;

namespace InventoryManagement.Application.Features.Companies.Commands.DeleteCompany
{
    public sealed record DeleteCompanyCommand(int Id) : IRequest<bool>;
}
