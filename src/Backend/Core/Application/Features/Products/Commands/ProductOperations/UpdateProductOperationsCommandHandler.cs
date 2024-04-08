using Application.Interfaces.Repositories;
using Domain.Entities;
using Application.Common.Exceptions;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using SharedLibrary.Common;
using WorkflowCore.Interface;

namespace Application.Features.Products.Commands.ProductOperations
{
    internal class UpdateProductOperationsCommandHandler : IRequestHandler<UpdateProductOperationsCommand, Result<Product>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateProductOperationsCommandHandler> _logger;
        private readonly IWorkflowHost _workflowHost;
        private readonly IWorkflowRepository _workflowRepository;

        public UpdateProductOperationsCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductOperationsCommandHandler> logger,
            IWorkflowHost workflowHost, IWorkflowRepository workflowRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _workflowHost = workflowHost;
            _workflowRepository = workflowRepository;
        }



        public async Task<Result<Product>> Handle(UpdateProductOperationsCommand request, CancellationToken cancellationToken)
        {
            Company senderCompany = null;
            Company recipientCompany = null;


            // Ürün kontrolü
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
            if (product == null)
            {
                _logger.LogWarning($"Product Id not found: {request.Id}");
                throw new NotFoundExceptionCustom($"Ürün bulunamadı");
            }
            if (product.Status == "Transfer Aşamasında" && request.TypeOfOperations == GenericConstantDefinitions.Transfer)
            {
                _logger.LogWarning($"The product is already in transit : {request.Id}");
                throw new BadRequestExceptionCustom($"{request.Id} id numaralı ürün zaten transfer aşamasında");
            }


            if (request.TypeOfOperations == GenericConstantDefinitions.Transfer)
            {
                // Şirketleri aynı sorguda al
                var companyIds = new List<int> { product.CompanyId, request.RecipientCompanyId.Value };
                var companies = await _unitOfWork.Repository<Company>().Entities
                    .Where(x => companyIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id);

                if (!companies.TryGetValue(product.CompanyId, out senderCompany))
                {
                    _logger.LogWarning($"Sender Company Id not found: {product.CompanyId}");
                    throw new NotFoundExceptionCustom($"{request.Id} için gönderilecek şirket bilgisi bulunamadı");
                }

                if (!companies.TryGetValue(request.RecipientCompanyId.Value, out recipientCompany))
                {
                    _logger.LogWarning($"Recipient Company Id not found: {request.RecipientCompanyId}");
                    throw new NotFoundExceptionCustom($"Alıcı şirket bulunamadı");
                }
            }



            string typeOfOperation = request.TypeOfOperations;
            string changeStatus = typeOfOperation switch
            {
                GenericConstantDefinitions.Transfer => GenericConstantDefinitions.Transfer,
                GenericConstantDefinitions.Accepted => GenericConstantDefinitions.Accepted,
                GenericConstantDefinitions.Rejected => GenericConstantDefinitions.Rejected,
                GenericConstantDefinitions.ReturnIt => GenericConstantDefinitions.ReturnIt,
                _ => throw new ArgumentOutOfRangeException(nameof(typeOfOperation))
            };


            var productTransferDto = new UpdateProductOperationsCommand
            {
                Id = request.Id,
                TypeOfOperations = request?.TypeOfOperations,
                UpdatedBy = request?.UpdatedBy,
                UpdatedUserId = request?.UpdatedUserId,
                RecipientCompanyName = recipientCompany?.Name,
                RecipientCompanyId = request?.RecipientCompanyId,
                RecipientEmail = request?.RecipientEmail,
                RecipientUserName = request?.RecipientUserName,
                SenderCompanyName = senderCompany?.Name,
                SenderEmail = request?.SenderEmail,
                SenderUserName = request?.SenderUserName
            };

            #region Workflow -> Product Tablo işlemleri
            //Transfer
            if (request.TypeOfOperations == GenericConstantDefinitions.Transfer)
            {
                product.WorkflowId = await _workflowHost.StartWorkflow("ProductTransferWorkflow", 1, productTransferDto);
            }
            //İade
            else if (request.TypeOfOperations == GenericConstantDefinitions.ReturnIt)
            {
                var result = _workflowHost.PublishEvent("Inventory_Approval_Decision", product.WorkflowId, productTransferDto.TypeOfOperations);

                var executionPointer = _workflowRepository.GetWorkflowInstance(product.WorkflowId).Result.Data.Adapt<UpdateProductOperationsCommand>();
                productTransferDto.RecipientCompanyId = executionPointer.RecipientCompanyId;
                productTransferDto.RecipientCompanyName = executionPointer.RecipientCompanyName;
                productTransferDto.RecipientUserName = executionPointer.RecipientUserName;
                productTransferDto.RecipientEmail = executionPointer.RecipientEmail;

                productTransferDto.SenderCompanyName = executionPointer.SenderCompanyName;
                productTransferDto.SenderUserName = executionPointer.SenderUserName;
                productTransferDto.SenderEmail = executionPointer.SenderEmail;
                product.Status = GenericConstantDefinitions.ReturnIt;
            }
            //Red edildi
            else if (request.TypeOfOperations == GenericConstantDefinitions.Rejected)
            {
                var result = _workflowHost.PublishEvent("Inventory_Approval_Decision", product.WorkflowId, productTransferDto.TypeOfOperations);

                var executionPointer = _workflowRepository.GetWorkflowInstance(product.WorkflowId).Result.Data.Adapt<UpdateProductOperationsCommand>();
                productTransferDto.RecipientCompanyId = executionPointer.RecipientCompanyId;
                productTransferDto.RecipientCompanyName = executionPointer.RecipientCompanyName;
                productTransferDto.RecipientUserName = executionPointer.RecipientUserName;
                productTransferDto.RecipientEmail = executionPointer.RecipientEmail;

                productTransferDto.SenderCompanyName = executionPointer.SenderCompanyName;
                productTransferDto.SenderUserName = executionPointer.SenderUserName;
                productTransferDto.SenderEmail = executionPointer.SenderEmail;

                product.Status = GenericConstantDefinitions.Rejected;
            }

            else if (request.TypeOfOperations == GenericConstantDefinitions.Accepted)
            {
                var result = _workflowHost.PublishEvent("Inventory_Approval_Decision", product.WorkflowId, GenericConstantDefinitions.Accepted);
                var executionPointer = _workflowRepository.GetWorkflowInstance(product.WorkflowId).Result.Data.Adapt<UpdateProductOperationsCommand>();

                //product.status == "Transfer Aşamasında1
                if (product.Status == GenericConstantDefinitions.Transfer && request.TypeOfOperations == GenericConstantDefinitions.Accepted)
                {
                    product.CompanyId = executionPointer.RecipientCompanyId.Value;
                    product.Status = GenericConstantDefinitions.InStock;
                    product.WorkflowId = null;
                }
            }
            #endregion

            product.Status = changeStatus;

            product.AddDomainEvent(new ProductOperationsUpdatedEvent(productTransferDto));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await Result<Product>.SuccessAsync(product);
        }
    }
}
