using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Brands.Commands.DeleteBrand
{
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBrandCommandHandler> _logger;
        private readonly IEasyCacheService _easyCacheService;

        public DeleteBrandCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteBrandCommandHandler> logger, IEasyCacheService easyCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _easyCacheService = easyCacheService;
        }

        public async Task<bool> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(request.Id);
            if (brand == null)
            {
                _logger.LogWarning($"Brand Id not found: {request.Id}");
                throw new NotFoundExceptionCustom($"Marka bulunamadı");
            }


            // Şirketi veritabanından silmeden önce Redis'ten kaldır
            await _easyCacheService.RemoveAsync($"Brand_{brand.Id}");

            await _unitOfWork.Repository<Brand>().DeleteAsync(brand);
            brand.AddDomainEvent(new BrandDeletedEvent(brand));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
