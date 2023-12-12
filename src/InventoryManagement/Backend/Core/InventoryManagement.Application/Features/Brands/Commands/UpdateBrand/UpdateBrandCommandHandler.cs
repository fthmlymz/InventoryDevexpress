using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Brands.Commands.UpdateBrand
{
    internal class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Result<Brand>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBrandCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public UpdateBrandCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateBrandCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<Brand>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            // EasyCache kontrolü yap
            var cacheKey = $"Category_{request.Id}";
            var cachedBrand = await _easyCacheService.GetAsync(cacheKey, typeof(Brand));
            if (cachedBrand != null)
            {
                var cacheBrand = (Brand)cachedBrand;
                // Önbellekte var olan kategori bilgisini güncelleme talebiyle karşılaştır
                bool isUpToDate = true;
                foreach (var propertyInfo in request.GetType().GetProperties())
                {
                    var value = propertyInfo.GetValue(request);
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        var propertyName = propertyInfo.Name;
                        var cachePropertyValue = cacheBrand.GetType().GetProperty(propertyName)?.GetValue(cacheBrand);

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
                    _logger.LogInformation($"Brand with Id {request.Id} already up to date. Returning cached result.");
                    return await Result<Brand>.SuccessAsync(cacheBrand);
                }
            }


            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(request.Id);
            if (brand == null)
            {
                _logger.LogWarning($"Category Id not found: {request.Id}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} isimli kategori bulunamadı");
            }


            //Company check
            if (request.CompanyId > 0)
            {
                var company = _unitOfWork.Repository<Company>().Entities.FirstOrDefault(x => x.Id == request.CompanyId);
                if (company == null)
                {
                    _logger.LogWarning($"Company ID not found: {request.Name}", request.Name);
                    throw new NotFoundExceptionCustom($"{request.Name} için kayıt edilecek şirket bilgisi bulunamadu");
                }
            }


            // Brand bilgisini güncelle
            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var brandProperty = brand.GetType().GetProperty(propertyName);
                    brandProperty.SetValue(brand, value);
                }
            }



            await _unitOfWork.Repository<Brand>().UpdateAsync(brand);
            brand.AddDomainEvent(new BrandUpdatedEvent(brand));
            await _unitOfWork.SaveChangesAsync(cancellationToken);




            // Döngüyü kırmak için Company nesnesini null'a atayalım(Relationship hatasını önlemek için)
            brand.Company = null;


            // Güncellenen category önbelleğe al
            await _easyCacheService.SetAsync(cacheKey, brand);
            return await Result<Brand>.SuccessAsync(brand);
        }
    }
}
