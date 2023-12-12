using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;



namespace InventoryManagement.Application.Features.Companies.Commands.UpdateCompany
{
    internal class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<Company>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCompanyCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public UpdateCompanyCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCompanyCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<Company>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            // EasyCache kontrolü yap
            var cacheKey = $"Company_{request.Id}";
            var cachedCompany = await _easyCacheService.GetAsync(cacheKey, typeof(Company));
            if (cachedCompany != null)
            {
                var cacheCompany = (Company)cachedCompany;
                // Önbellekte var olan şirket bilgisini güncelleme talebiyle karşılaştır
                bool isUpToDate = true;
                foreach (var propertyInfo in request.GetType().GetProperties())
                {
                    var value = propertyInfo.GetValue(request);
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        var propertyName = propertyInfo.Name;
                        var cachePropertyValue = cacheCompany.GetType().GetProperty(propertyName)?.GetValue(cacheCompany);

                        // İlgili özelliğin değeri değiştiyse güncelleme yap
                        if (cachePropertyValue == null || !cachePropertyValue.Equals(value))
                        {
                            isUpToDate = false;
                            await _easyCacheService.SetAsync(cacheKey, request); // Yeni değeri önbelleğe ekle
                            break;
                        }
                    }
                }
                if (isUpToDate)
                {
                    _logger.LogInformation($"Company with Id {request.Id} already up to date. Returning cached result.");
                    return await Result<Company>.SuccessAsync(cacheCompany);
                }
            }


            var company = await _unitOfWork.Repository<Company>().GetByIdAsync(request.Id);
            if (company == null)
            {
                _logger.LogWarning($"Company Id not found {request.Id}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} isimli şirket bulunamadı.");
            }

            // Şirket bilgilerini güncelle
            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var companyProperty = company.GetType().GetProperty(propertyName);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    companyProperty.SetValue(company, value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
            }


            await _unitOfWork.Repository<Company>().UpdateAsync(company);
            company.AddDomainEvent(new CompanyUpdatedEvent(company));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Güncellenen şirketi önbelleğe al
            await _easyCacheService.SetAsync(cacheKey, company);
            return await Result<Company>.SuccessAsync(company);
        }
    }
}
