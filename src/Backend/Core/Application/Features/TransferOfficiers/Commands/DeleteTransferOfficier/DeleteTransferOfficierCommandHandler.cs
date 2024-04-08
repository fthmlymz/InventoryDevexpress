using Application.Interfaces.Repositories;
using Domain.Entities;
using Application.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.TransferOfficiers.Commands.DeleteTransferOfficier
{
    internal class DeleteTransferOfficierCommandHandler : IRequestHandler<DeleteTransferOfficierCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteTransferOfficierCommandHandler> _logger;

        public DeleteTransferOfficierCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteTransferOfficierCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteTransferOfficierCommand request, CancellationToken cancellationToken)
        {
            var transferOfficier = await _unitOfWork.Repository<TransferOfficier>().GetByIdAsync(request.Id);
            if (transferOfficier == null)
            {
                _logger.LogWarning($"Transfer information not found: {request.Id}");
                throw new NotFoundExceptionCustom($"Transfer bilgisi bulunamadı {request.Id}");
            }

            await _unitOfWork.Repository<TransferOfficier>().DeleteAsync(transferOfficier);
            transferOfficier.AddDomainEvent(new TransferOfficierDeletedEvent(transferOfficier));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
