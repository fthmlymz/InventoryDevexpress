using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Application.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.Categories.Commands.UpdateCategory
{
    internal class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<Category>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCategoryCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;

            // MapsterConfig konfigürasyonunu yap
            //TypeAdapterConfig<Category, Category>.NewConfig();
        }

        public async Task<Result<Category>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // EasyCache kontrolü yap
            var cacheKey = $"Category_{request.Id}";
            var cachedCategory = await _easyCacheService.GetAsync(cacheKey, typeof(Category));
            if (cachedCategory != null)
            {
                var cacheCategory = (Category)cachedCategory;
                // Önbellekte var olan kategori bilgisini güncelleme talebiyle karşılaştır
                bool isUpToDate = true;
                foreach (var propertyInfo in request.GetType().GetProperties())
                {
                    var value = propertyInfo.GetValue(request);
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        var propertyName = propertyInfo.Name;
                        var cachePropertyValue = cacheCategory.GetType().GetProperty(propertyName)?.GetValue(cacheCategory);

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
                    _logger.LogInformation($"Category with Id {request.Id} already up to date. Returning cached result.");
                    return await Result<Category>.SuccessAsync(cacheCategory);
                }
            }


            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.Id);
            if (category == null)
            {
                _logger.LogWarning($"Category Id not found: {request.Id}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} isimli kategori bulunamadı");
            }

            // Kategori önbellekte bulunamadı, veritabanına git
            var categoryExists = await _unitOfWork.Repository<Category>().AnyAsync(x => x.Name == request.Name);
            if (categoryExists)
            {
                _logger.LogWarning($"Already registered with this name: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli kategori daha önce kayıt edilmiş.");
            }

            /*var company = _unitOfWork.Repository<Company>().Entities.FirstOrDefault(x => x.Id == request.CompanyId);
            if (company == null)
            {
                _logger.LogWarning($"Company ID not found: {request.Name}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} için kayıt edilecek şirket bilgisi bulunamadu");
            }*/

            // Category bilgisini güncelle
            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var categoryProperty = category.GetType().GetProperty(propertyName);
                    categoryProperty.SetValue(category, value);
                }
            }

            await _unitOfWork.Repository<Category>().UpdateAsync(category);
            category.AddDomainEvent(new CategoryUpdatedEvent(category));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Güncellenen category önbelleğe al
            await _easyCacheService.SetAsync(cacheKey, category);
            return await Result<Category>.SuccessAsync(category);
        }
    }
}
