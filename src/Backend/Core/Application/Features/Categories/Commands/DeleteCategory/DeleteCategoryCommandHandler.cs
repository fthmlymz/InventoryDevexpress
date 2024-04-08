using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Application.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Commands.DeleteCategory
{
    //Best performance
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.Id);
            if (category == null)
            {
                _logger.LogWarning($"Category Id not found: {request.Id}");
                throw new NotFoundExceptionCustom($"Kategori bulunamadı");
            }

            // Şirketi veritabanından silmeden önce Redis'ten kaldır
            await _easyCacheService.RemoveAsync($"Category_{category.Id}");

            await _unitOfWork.Repository<Category>().DeleteAsync(category);
            category.AddDomainEvent(new CategoryDeletedEvent(category));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
