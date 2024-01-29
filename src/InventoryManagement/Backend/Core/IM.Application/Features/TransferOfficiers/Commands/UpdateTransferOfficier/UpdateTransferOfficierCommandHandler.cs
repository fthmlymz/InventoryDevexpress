using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.TransferOfficiers.Commands.UpdateTransferOfficier
{
    internal class UpdateTransferOfficierCommandHandler : IRequestHandler<UpdateTransferOfficierCommand, Result<TransferOfficier>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateTransferOfficierCommandHandler> _logger;

        public UpdateTransferOfficierCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateTransferOfficierCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<TransferOfficier>> Handle(UpdateTransferOfficierCommand request, CancellationToken cancellationToken)
        {
            var transferOfficier = await _unitOfWork.Repository<TransferOfficier>().GetByIdAsync(request.Id);
            if (transferOfficier == null)
            {
                _logger.LogWarning($"Transfer responsible ID number not found: {request.Id}", request.UserName);
                throw new NotFoundExceptionCustom($"{request.Id} numaralı transfer sorumlusu bulunamadı");
            }

            //TransferOfficier için kayıt edilecek company bilgisi
            var company = _unitOfWork.Repository<Company>().Entities.FirstOrDefault(x => x.Id == request.CompanyId);
            if (company == null)
            {
                _logger.LogWarning($"Company ID not found: {request.UserName}", request.UserName);
                throw new NotFoundExceptionCustom($"{request.UserName} için kayıt edilecek şirket bilgisi bulunamadu");
            }


            // Trans bilgisini güncelle
            foreach (var propertyInfo in request.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(request);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var propertyName = propertyInfo.Name;
                    var productProperty = transferOfficier.GetType().GetProperty(propertyName);
                    productProperty.SetValue(transferOfficier, value);
                }
            }

            await _unitOfWork.Repository<TransferOfficier>().UpdateAsync(transferOfficier);
            transferOfficier.AddDomainEvent(new TransferOfficierUpdatedEvent(transferOfficier));
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return await Result<TransferOfficier>.SuccessAsync(transferOfficier);
        }
    }
}
