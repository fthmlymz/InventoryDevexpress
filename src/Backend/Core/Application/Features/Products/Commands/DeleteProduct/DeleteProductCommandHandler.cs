using Application.Interfaces.Repositories;
using Domain.Entities;
using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Features.Products.Commands.DeleteProduct;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
            if (product == null)
            {
                _logger.LogWarning($"Product Id not found: {request.Id}");
                throw new NotFoundExceptionCustom($"Ürün bulunamadı {request.Id}");
            }

            await _unitOfWork.Repository<Product>().DeleteAsync(product);
            product.AddDomainEvent(new ProductDeletedEvent(product));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
