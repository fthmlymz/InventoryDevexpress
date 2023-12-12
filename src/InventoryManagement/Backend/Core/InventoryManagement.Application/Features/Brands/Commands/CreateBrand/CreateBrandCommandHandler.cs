using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Brands.Commands.CreateBrand
{
    internal sealed class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Result<CreatedBrandDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateBrandCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public CreateBrandCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateBrandCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<CreatedBrandDto>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            //Easycache'te brand ara
            var cacheKey = $"Brand_{request.Name}";
            var cachedBrand = await _easyCacheService.GetAsync<Brand>(cacheKey);
            if (cachedBrand != null)
            {
                var createdDto = cachedBrand.Adapt<CreatedBrandDto>();
                return await Result<CreatedBrandDto>.SuccessAsync(createdDto);
            }


            //Brand önbellekte bulunamadı, veritabanına git
            var brandExists = await _unitOfWork.Repository<Brand>().AnyAsync(x => x.Name == request.Name);
            if (brandExists)
            {
                _logger.LogWarning($"Already registered with this name: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli marka daha önce kayıt edilmiş.");
            }



            //Şirket kayıtlı mı ?
            var companyExists = _unitOfWork.Repository<Company>().Entities.SingleOrDefault(x => x.Id == request.CompanyId);
            if (companyExists == null)
            {
                _logger.LogWarning($"Company Id not found for brand record: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} için kayıt edilecek şirket bilgisi bulunamadı");
            }


            var brand = request.Adapt<Brand>();
            await _unitOfWork.Repository<Brand>().AddAsync(brand);
            brand.AddDomainEvent(new BrandCreatedEvent(brand));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Döngüyü kırmak için Company nesnesini null'a atayalım(Relationship hatasını önlemek için)
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            brand.Company = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            //Tüm tabloyu önbelleğe kaydet
            cacheKey = $"Brand_{brand.Id}";
            await _easyCacheService.SetAsync(cacheKey, brand);

            var createdBrandDto = brand.Adapt<CreatedBrandDto>();
            return await Result<CreatedBrandDto>.SuccessAsync(createdBrandDto);
        }
    }
}
