using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.CategoriesSub.Commands.DeleteCategorySub
{
    public class DeleteCategorySubCommandHandler : IRequestHandler<DeleteCategorySubCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategorySubCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public DeleteCategorySubCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategorySubCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }


        public async Task<bool> Handle(DeleteCategorySubCommand request, CancellationToken cancellationToken)
        {
            var categorySub = await _unitOfWork.Repository<CategorySub>().GetByIdAsync(request.Id);
            if (categorySub == null)
            {
                _logger.LogWarning($"CategorySub Id not found: {request.Id}");
                throw new NotFoundExceptionCustom($"Alt kategori bulunamadı");
            }

            // Şirketi veritabanından silmeden önce Redis'ten kaldır
            await _easyCacheService.RemoveAsync($"CategorySub_{categorySub.Id}");

            await _unitOfWork.Repository<CategorySub>().DeleteAsync(categorySub);
            categorySub.AddDomainEvent(new CategoryDeletedEvent(categorySub));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
