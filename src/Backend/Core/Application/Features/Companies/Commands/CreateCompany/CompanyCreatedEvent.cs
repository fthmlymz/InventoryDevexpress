using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;
using MediatR;

namespace InventoryManagement.Application.Features.Companies.Commands.CreateCompany
{
    public class CompanyCreatedEvent : BaseEvent, INotification
    {
        public Company Company { get; }
        public CompanyCreatedEvent(Company company)
        {
            Company = company;
        }
    }
}
