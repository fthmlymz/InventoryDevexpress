using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Models.Commands.UpdateModel
{
    internal class UpdateModelCommandHandler : IRequestHandler<UpdateModelCommand, Result<Model>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateModelCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public UpdateModelCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateModelCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<Result<Model>> Handle(UpdateModelCommand request, CancellationToken cancellationToken)
        {
            // EasyCache kontrolü yap
            var cacheKey = $"Model_{request.Id}";
            var cachedModel = await _easyCacheService.GetAsync(cacheKey, typeof(Model));
            if (cachedModel != null)
            {
                var cacheModel = (Model)cachedModel;
                // Önbellekte var olan model bilgisini güncelleme talebiyle karşılaştır
                bool isUpToDate = true;
                foreach (var propertyInfo in request.GetType().GetProperties())
                {
                    var value = propertyInfo.GetValue(request);
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        var propertyName = propertyInfo.Name;
                        var cachePropertyValue = cacheModel.GetType().GetProperty(propertyName)?.GetValue(cacheModel);

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
                    _logger.LogInformation($"Model with Id {request.Id} already up to date. Returning cached result.");
                    return await Result<Model>.SuccessAsync(cacheModel);
                }
            }



            //Model kontrolü
            var model = await _unitOfWork.Repository<Model>().GetByIdAsync(request.Id);
            if (model == null)
            {
                _logger.LogWarning($"Model Id not found: {request.Id}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} isimli model bulunamadı");
            }



            // Model önbellekte bulunamadı, veritabanına git
            var modelExists = await _unitOfWork.Repository<Model>().AnyAsync(x => x.Name == request.Name);
            if (modelExists)
            {
                _logger.LogWarning($"Already registered with this name: {request.Name}", request.Name);
                throw new BadRequestExceptionCustom($"{request.Name} isimli model daha önce kayıt edilmiş.");
            }


            var brand = _unitOfWork.Repository<Brand>().Entities.FirstOrDefault(x => x.Id == request.BrandId);
            if (brand == null)
            {
                _logger.LogWarning($"Brand ID not found: {request.Name}", request.Name);
                throw new NotFoundExceptionCustom($"{request.Name} için kayıt edilecek marka bilgisi bulunamadu");
            }


            // Model bilgisini güncelle
            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var modelProperty = model.GetType().GetProperty(propertyName);
                    modelProperty.SetValue(model, value);
                }
            }

            await _unitOfWork.Repository<Model>().UpdateAsync(model);
            model.AddDomainEvent(new ModelUpdatedEvent(model));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Döngüyü kırmak için Brand nesnesini null'a atayalım(Relationship hatasını önlemek için)
            model.Brand = null;

            // Güncellenen category önbelleğe al
            await _easyCacheService.SetAsync(cacheKey, model);
            return await Result<Model>.SuccessAsync(model);
        }
    }
}
