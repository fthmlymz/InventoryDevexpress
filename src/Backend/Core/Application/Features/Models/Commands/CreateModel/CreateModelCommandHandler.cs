using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using InventoryManagement.Application.Common.Exceptions;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.Models.Commands.CreateModel
{
    internal sealed class CreateModelCommandHandler : IRequestHandler<CreateModelCommand, Result<CreatedModelDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateModelCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public CreateModelCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateModelCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<CreatedModelDto>> Handle(CreateModelCommand request, CancellationToken cancellationToken)
        {
            // Easycache'te model kategoriyi ara
            var cacheKey = $"Model_{request.Name}";
            var cachedModel = await _easyCacheService.GetAsync<Model>(cacheKey);
            if (cachedModel != null)
            {
                // Alt Kategori önbellekte bulundu, istenen işlemleri gerçekleştirme
                var createdDto = cachedModel.Adapt<CreatedModelDto>();
                return await Result<CreatedModelDto>.SuccessAsync(createdDto);
            }



            // Model önbellekte bulunamadı, veritabanına git
            var modelExists = await _unitOfWork.Repository<Model>().AnyAsync(x => x.Name == request.Name);
            if (modelExists)
            {
                _logger.LogWarning($"Already registered with this name: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli model daha önce kayıt edilmiş.");
            }


            //Brand kayıtlı mı ?
            var brandExists = _unitOfWork.Repository<Brand>().Entities.SingleOrDefault(x => x.Id == request.BrandId);
            if (brandExists == null)
            {
                _logger.LogWarning($"Brand Id not found for model record: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} için kayıt edilecek ana marka bilgisi bulunamadı");
            }


            var model = request.Adapt<Model>();
            await _unitOfWork.Repository<Model>().AddAsync(model);
            model.AddDomainEvent(new CreateModelEvent(model));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Döngüyü kırmak için Brand nesnesini null'a atayalım(Relationship hatasını önlemek için)
            model.Brand = null;


            // Tüm tabloyu önbelleğe kaydet
            cacheKey = $"Model_{model.Id}";
            await _easyCacheService.SetAsync(cacheKey, model);

            var createdModelDto = model.Adapt<CreatedModelDto>();
            return await Result<CreatedModelDto>.SuccessAsync(createdModelDto);
        }
    }
}
