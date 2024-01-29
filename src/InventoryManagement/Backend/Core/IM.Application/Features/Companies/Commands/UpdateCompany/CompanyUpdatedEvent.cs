using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;
using MediatR;

namespace InventoryManagement.Application.Features.Companies.Commands.UpdateCompany
{
    public class CompanyUpdatedEvent : BaseEvent, INotification
    {
        public Company Company { get; }
        public CompanyUpdatedEvent(Company company)
        {
            Company = company;
        }
    }
}
