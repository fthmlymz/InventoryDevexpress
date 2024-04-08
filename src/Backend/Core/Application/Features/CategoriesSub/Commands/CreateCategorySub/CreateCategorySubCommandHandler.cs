using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Application.Common.Exceptions;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.CategoriesSub.Commands.CreateCategorySub
{
    internal sealed class CreateCategorySubCommandHandler : IRequestHandler<CreateCategorySubCommand, Result<CreatedCategorySubDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCategorySubCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public CreateCategorySubCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCategorySubCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<CreatedCategorySubDto>> Handle(CreateCategorySubCommand request, CancellationToken cancellationToken)
        {
            // Easycache'te alt kategoriyi ara
            var cacheKey = $"CategorySub_{request.Name}";
            var cachedCategorySub = await _easyCacheService.GetAsync<CategorySub>(cacheKey);
            if (cachedCategorySub != null)
            {
                // Alt Kategori önbellekte bulundu, istenen işlemleri gerçekleştirme
                var createdDto = cachedCategorySub.Adapt<CreatedCategorySubDto>();
                return await Result<CreatedCategorySubDto>.SuccessAsync(createdDto);
            }

            // Kategori önbellekte bulunamadı, veritabanına git
            var categorySubExists = await _unitOfWork.Repository<CategorySub>().AnyAsync(x => x.Name == request.Name);
            if (categorySubExists)
            {
                _logger.LogWarning($"Already registered with this name: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli alt kategori daha önce kayıt edilmiş.");
            }


            // Ana Kategori kayıtlı mı ?
            var categoryExists = _unitOfWork.Repository<Category>().Entities.SingleOrDefault(x => x.Id == request.CategoryId);
            if (categoryExists == null)
            {
                _logger.LogWarning($"Category Id not found for categorysub record: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} için kayıt edilecek ana kategori bilgisi bulunamadı");
            }


            var categorySub = request.Adapt<CategorySub>();
            await _unitOfWork.Repository<CategorySub>().AddAsync(categorySub);
            categorySub.AddDomainEvent(new CreateCategorySubEvent(categorySub));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Döngüyü kırmak için Company nesnesini null'a atayalım(Relationship hatasını önlemek için)
            categorySub.Category = null;


            // Tüm tabloyu önbelleğe kaydet
            cacheKey = $"CategorySub_{categorySub.Id}"; // Cache key'i güncelle
            await _easyCacheService.SetAsync(cacheKey, categorySub);

            var createdCategorySubDto = categorySub.Adapt<CreatedCategorySubDto>();
            return await Result<CreatedCategorySubDto>.SuccessAsync(createdCategorySubDto);
        }
    }
}
