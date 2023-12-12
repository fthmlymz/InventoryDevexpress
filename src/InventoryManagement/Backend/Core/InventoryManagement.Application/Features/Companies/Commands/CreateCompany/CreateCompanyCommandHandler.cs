using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Companies.Commands.CreateCompany
{
    internal sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CreatedCompanyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCompanyCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public CreateCompanyCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCompanyCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<CreatedCompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            // Easycache'te şirketi ara
            var cacheKey = $"Company_{request.Name}";
            var cachedCompany = await _easyCacheService.GetAsync<Company>(cacheKey);
            if (cachedCompany != null)
            {
                // Şirket önbellekte bulundu, istenen işlemleri gerçekleştirme
                var createdDto = cachedCompany.Adapt<CreatedCompanyDto>();
                return await Result<CreatedCompanyDto>.SuccessAsync(createdDto);
            }

            // Şirket önbellekte bulunamadı, veritabanına git
            var companyExist = await _unitOfWork.Repository<Company>().AnyAsync(x => x.Name == request.Name);
            if (companyExist)
            {
                _logger.LogWarning("Already registered with this name: {RequestName}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli şirket daha önce kayıt edilmiş.");
            }

            var company = request.Adapt<Company>();
            await _unitOfWork.Repository<Company>().AddAsync(company);
            company.AddDomainEvent(new CompanyCreatedEvent(company));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Tüm tabloyu önbelleğe kaydet
            cacheKey = $"Company_{company.Id}"; // Cache key'i güncelle
            await _easyCacheService.SetAsync(cacheKey, company);


            var createdCompanyDto = company.Adapt<CreatedCompanyDto>();
            return await Result<CreatedCompanyDto>.SuccessAsync(createdCompanyDto);
        }
    }
}
