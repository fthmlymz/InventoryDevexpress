using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;
using MediatR;

namespace InventoryManagement.Application.Features.Companies.Commands.DeleteCompany
{
    public class CompanyDeletedEvent : BaseEvent, INotification
    {
        public Company Company { get; }
        public CompanyDeletedEvent(Company company)
        {
            Company = company;
        }
    }
}
