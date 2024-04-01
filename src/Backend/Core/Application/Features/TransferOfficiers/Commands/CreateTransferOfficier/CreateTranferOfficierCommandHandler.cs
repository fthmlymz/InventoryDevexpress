using Application.Interfaces.Repositories;
using Domain.Entities;
using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Features.TransferOfficiers.Commands.CreateTransferOfficier;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Features.TransferOfficiers.Commands.CreateTransferOfficier
{
    internal sealed class CreateTranferOfficierCommandHandler : IRequestHandler<CreateTranferOfficierCommand, Result<TransferOfficierDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateTranferOfficierCommandHandler> _logger;

        public CreateTranferOfficierCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateTranferOfficierCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<TransferOfficierDto>> Handle(CreateTranferOfficierCommand request, CancellationToken cancellationToken)
        {
            //Şirket kayıtlı mı ?
            var companyExists = _unitOfWork.Repository<Company>().Entities.SingleOrDefault(x => x.Id == request.CompanyId);
            if (companyExists == null)
            {
                _logger.LogWarning($"Company Id not found for product record: {request.CompanyId}", request.CompanyId);
                throw new BadRequestExceptionCustom($"{request.CompanyId} için kayıt edilecek şirket bilgisi bulunamadı");
            }


            var data = request.Adapt<TransferOfficier>();
            await _unitOfWork.Repository<TransferOfficier>().AddAsync(data);
            data.AddDomainEvent(new TransferOfficierCreatedEvent(data));
            await _unitOfWork.SaveChangesAsync(cancellationToken);



            return await Result<TransferOfficierDto>.SuccessAsync(data.Adapt<TransferOfficierDto>());
        }
    }
}
