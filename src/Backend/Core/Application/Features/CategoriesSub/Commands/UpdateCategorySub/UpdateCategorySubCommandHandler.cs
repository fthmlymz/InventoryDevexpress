using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Application.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.CategoriesSub.Commands.UpdateCategorySub
{
    internal class UpdateCategorySubCommandHandler : IRequestHandler<UpdateCategorySubCommand, Result<CategorySub>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCategorySubCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public UpdateCategorySubCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCategorySubCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;

            //TypeAdapterConfig<CategorySub, CategorySub>.NewConfig();
        }

        public async Task<Result<CategorySub>> Handle(UpdateCategorySubCommand request, CancellationToken cancellationToken)
        {
            // EasyCache kontrolü yap
            var cacheKey = $"CategorySub_{request.Id}";
            var cachedCategorySub = await _easyCacheService.GetAsync(cacheKey, typeof(CategorySub));
            if (cachedCategorySub != null)
            {
                var cacheCategorySub = (CategorySub)cachedCategorySub;
                // Önbellekte var olan kategori bilgisini güncelleme talebiyle karşılaştır
                bool isUpToDate = true;
                foreach (var propertyInfo in request.GetType().GetProperties())
                {
                    var value = propertyInfo.GetValue(request);
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        var propertyName = propertyInfo.Name;
                        var cachePropertyValue = cacheCategorySub.GetType().GetProperty(propertyName)?.GetValue(cacheCategorySub);

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
                    _logger.LogInformation($"CategorySub with Id {request.Id} already up to date. Returning cached result.");
                    return await Result<CategorySub>.SuccessAsync(cacheCategorySub);
                }
            }

            // Alt kategori kontrolü
            var categorySub = await _unitOfWork.Repository<CategorySub>().GetByIdAsync(request.Id);
            if (categorySub == null)
            {
                _logger.LogWarning($"CategorySub Id not found: {request.Id}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} isimli alt kategori bulunamadı");
            }

            var category = _unitOfWork.Repository<Category>().Entities.FirstOrDefault(x => x.Id == request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning($"Category ID not found: {request.Name}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} için kayıt edilecek ana kategori bilgisi bulunamadu");
            }

            // Kategori önbellekte bulunamadı, veritabanına git
            var categorySubExists = await _unitOfWork.Repository<CategorySub>().AnyAsync(x => x.Name == request.Name);
            if (categorySubExists)
            {
                _logger.LogWarning($"Already registered with this name: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli alt kategori daha önce kayıt edilmiş.");
            }

            // CategorySub bilgisini güncelle
            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var categorySubProperty = categorySub.GetType().GetProperty(propertyName);
                    categorySubProperty.SetValue(categorySub, value);
                }
            }

            await _unitOfWork.Repository<CategorySub>().UpdateAsync(categorySub);
            categorySub.AddDomainEvent(new CategorySubUpdatedEvent(categorySub));
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            // Döngüyü kırmak için Category nesnesini null'a atayalım(Relationship hatasını önlemek için)
            categorySub.Category = null;

            // Güncellenen category önbelleğe al
            await _easyCacheService.SetAsync(cacheKey, categorySub);
            return await Result<CategorySub>.SuccessAsync(categorySub);
        }
    }
}
