using Application.Features.Products.GeneralDtos;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.Products.Commands.FileTransfer
{
    internal sealed class FileTransferCommandHandler : IRequestHandler<FileTransferCommand, Result<CreatedProductMovementDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FileTransferCommandHandler> _logger;

        public FileTransferCommandHandler(IUnitOfWork unitOfWork, ILogger<FileTransferCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CreatedProductMovementDto>> Handle(FileTransferCommand request, CancellationToken cancellationToken)
        {
            var createdProductMovementDto = request.Adapt<CreatedProductMovementDto>();
            var movement = new CreatedProductMovementDto
            {
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                CreatedUserId = request.CreatedUserId,
                Description = request.Description,
                MovementDate = DateTime.Now,
                ProductId = request.ProductId
            };

            var movementDto = movement.Adapt<ProductMovement>();
            await _unitOfWork.Repository<ProductMovement>().AddAsync(movementDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await Result<CreatedProductMovementDto>.SuccessAsync(createdProductMovementDto);
        }
    }
}
