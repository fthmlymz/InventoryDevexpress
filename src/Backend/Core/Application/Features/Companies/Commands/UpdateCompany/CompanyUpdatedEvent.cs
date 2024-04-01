using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Companies.Commands.UpdateCompany
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
