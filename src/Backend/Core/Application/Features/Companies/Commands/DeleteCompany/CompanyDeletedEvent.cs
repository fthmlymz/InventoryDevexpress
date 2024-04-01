using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Companies.Commands.DeleteCompany
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
