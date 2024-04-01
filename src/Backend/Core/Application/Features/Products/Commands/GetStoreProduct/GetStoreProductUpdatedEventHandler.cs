using InventoryManagement.Application.Features.FileManager.Commands;
using InventoryManagement.Application.Features.FileManager.Queries;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;

namespace InventoryManagement.Application.Features.Products.Commands.GetStoreProduct
{
    public class GetStoreProductUpdatedEventHandler : INotificationHandler<GetStoreProductUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetStoreProductUpdatedEventHandler> _logger;
        private readonly IWebHostEnvironment _environment;//sonradan eklendi
        private readonly IMediator _mediator; //sonradan eklendi


        public GetStoreProductUpdatedEventHandler(IUnitOfWork unitOfWork, ILogger<GetStoreProductUpdatedEventHandler> logger, IWebHostEnvironment environment, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _environment = environment;
            _mediator = mediator;
        }

        public async Task Handle(GetStoreProductUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var updatedProduct = notification.Product;

            #region Product Movement Add
            var productMovement = new ProductMovement
            {
                MovementDate = DateTime.Now,
                Description = $"{updatedProduct.UpdatedBy} tarafından Depoya alma işlemi yapıldı",
                ProductId = updatedProduct.Id,
                UpdatedBy = updatedProduct.UpdatedBy,
                UpdatedUserId = updatedProduct.UpdatedUserId,
                UpdatedDate = DateTime.Now,
                CreatedBy = updatedProduct.CreatedBy,
                CreatedUserId = updatedProduct.CreatedUserId,
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.Repository<ProductMovement>().AddAsync(productMovement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            #endregion


            #region AssignedProduct
            if (updatedProduct.Status == GenericConstantDefinitions.InStock)
            { //GetById yapılacak, firstasync kaldırılacak
                var assignedProduct = _unitOfWork.Repository<AssignedProduct>().Entities.FirstOrDefault(ap => ap.ProductId == updatedProduct.Id);

                if (assignedProduct != null)
                {
                    assignedProduct.AssignedUserId = null;
                    assignedProduct.AssignedUserName = null;
                    assignedProduct.AssignedUserPhoto = null;
                    assignedProduct.ApprovalStatus = null;
                    assignedProduct.FullName = null;
                    assignedProduct.UpdatedBy = assignedProduct.UpdatedBy;
                    assignedProduct.UpdatedUserId = assignedProduct.UpdatedUserId;
                    await _unitOfWork.Repository<AssignedProduct>().UpdateAsync(assignedProduct);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            #endregion


            #region Get Store Remove File
            await MoveFilesToHistory(updatedProduct.Barcode.ToString());
            #endregion
        }
        private async Task MoveFilesToHistory(string barcode)
        {
            var searchQuery = new SearchFileQuery(barcode);
            var searchResult = await _mediator.Send(searchQuery);

            if (searchResult.Succeeded)
            {
                foreach (var fileItem in searchResult.Data)
                {
                    var deleteCommand = new DeleteFileCommand(fileItem.FileName, fileItem.FolderName);
                    await _mediator.Send(deleteCommand);
                }
            }
        }
    }
}
