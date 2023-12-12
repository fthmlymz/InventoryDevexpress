using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Categories.Commands.CreateCategory
{
    internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CreatedCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCategoryCommandHandler> logger, IEasyCacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = cacheService;
        }

        public async Task<Result<CreatedCategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Easycache'te kategoriyi ara
            var cacheKey = $"Category_{request.Name}";
            var cachedCategory = await _easyCacheService.GetAsync<Category>(cacheKey);
            if (cachedCategory != null)
            {
                // Kategori önbellekte bulundu, istenen işlemleri gerçekleştirme
                var createdDto = cachedCategory.Adapt<CreatedCategoryDto>();
                return await Result<CreatedCategoryDto>.SuccessAsync(createdDto);
            }



            // Kategori önbellekte bulunamadı, veritabanına git
            var categoryExists = await _unitOfWork.Repository<Category>().AnyAsync(x => x.Name == request.Name);
            if (categoryExists)
            {
                _logger.LogWarning($"Already registered with this name: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli kategori daha önce kayıt edilmiş.");
            }



            //Şirket kayıtlı mı ?
            if (request.CompanyId > 0)
            {
                Console.WriteLine("Sıfırdan büyük geldi");
                var companyExists = _unitOfWork.Repository<Company>().Entities.SingleOrDefault(x => x.Id == request.CompanyId);
                if (companyExists == null)
                {
                    _logger.LogWarning($"Company Id not found for category record: {request.Name}", request.Name);
                    throw new BadRequestExceptionCustom($"{request.Name} için kayıt edilecek şirket bilgisi bulunamadı");
                }
            }


            var category = request.Adapt<Category>();
            await _unitOfWork.Repository<Category>().AddAsync(category);
            category.AddDomainEvent(new CategoryCreatedEvent(category));

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            // Döngüyü kırmak için Company nesnesini null'a atayalım(Relationship hatasını önlemek için)
            category.Company = null;


            // Tüm tabloyu önbelleğe kaydet
            cacheKey = $"Category_{category.Id}"; // Cache key'i güncelle
            await _easyCacheService.SetAsync(cacheKey, category);

            var createdCategoryDto = category.Adapt<CreatedCategoryDto>();
            return await Result<CreatedCategoryDto>.SuccessAsync(createdCategoryDto);
        }
    }
}
