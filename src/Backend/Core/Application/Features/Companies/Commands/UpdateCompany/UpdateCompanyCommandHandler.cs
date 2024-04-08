using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Application.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;



namespace Application.Features.Companies.Commands.UpdateCompany
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

            // Şirket önbellekte bulunamadı, veritabanına git
            var companyExist = await _unitOfWork.Repository<Company>().AnyAsync(x => x.Name == request.Name);
            if (companyExist)
            {
                _logger.LogWarning("Already registered with this name: {RequestName}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli şirket daha önce kayıt edilmiş.");
            }


            // Şirket bilgilerini güncelle
            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var companyProperty = company.GetType().GetProperty(propertyName);
                    companyProperty.SetValue(company, value);
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
