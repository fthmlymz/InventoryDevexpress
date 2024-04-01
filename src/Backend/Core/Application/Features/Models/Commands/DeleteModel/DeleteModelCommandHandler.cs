using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using InventoryManagement.Application.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Models.Commands.DeleteModel
{
    public class DeleteModelCommandHandler : IRequestHandler<DeleteModelCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteModelCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public DeleteModelCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteModelCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<bool> Handle(DeleteModelCommand request, CancellationToken cancellationToken)
        {
            var model = await _unitOfWork.Repository<Model>().GetByIdAsync(request.Id);
            if (model == null)
            {
                _logger.LogWarning($"Model Id not found: {request.Id}");
                throw new NotFoundExceptionCustom($"Model bulunamadı");
            }



            // Model'i veritabanından silmeden önce Redis'ten kaldır
            await _easyCacheService.RemoveAsync($"Model_{model.Id}");

            await _unitOfWork.Repository<Model>().DeleteAsync(model);
            model.AddDomainEvent(new ModelDeletedEvent(model));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
