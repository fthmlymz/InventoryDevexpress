using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Companies.Commands.CreateCompany
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
