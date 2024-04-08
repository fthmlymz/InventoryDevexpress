using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Application.Common.Exceptions;
//using Application.Features.Companies.Commands.DeleteCompany;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Companies.Commands.DeleteCompany
{
    internal class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCompanyCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;


        public DeleteCompanyCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCompanyCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<bool> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _unitOfWork.Repository<Company>().GetByIdAsync(request.Id);
            if (company == null)
            {
                _logger.LogWarning($"Unable to find ID: {request.Id}", request.Id);
                throw new NotFoundExceptionCustom($"{request.Id} isimli şirket bulunamadı.");
            }

            // Şirketi veritabanından silmeden önce Redis'ten kaldır
            await _easyCacheService.RemoveAsync($"Company_{company.Id}");


            await _unitOfWork.Repository<Company>().DeleteAsync(company);
            company.AddDomainEvent(new CompanyDeletedEvent(company));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}
