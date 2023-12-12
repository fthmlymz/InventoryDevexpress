using InventoryManagement.Application.Features.Products.Commands.CreateProduct;
using InventoryManagement.Application.Features.Products.Commands.DeleteProduct;
using InventoryManagement.Application.Features.Products.Commands.FileTransfer;
using InventoryManagement.Application.Features.Products.Commands.GetStoreProduct;
using InventoryManagement.Application.Features.Products.Commands.ProductOperations;
using InventoryManagement.Application.Features.Products.Commands.UpdateProduct;
using InventoryManagement.Application.Features.Products.GeneralDtos;
using InventoryManagement.Application.Features.Products.Queries.GetByIdProductAndDetailsQuery;
using InventoryManagement.Application.Features.Products.Queries.GetProductListWithPaginationQuery;
using InventoryManagement.Application.Features.Products.Queries.ProductSearchWithPagination;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [Authorize("ProductCreateRole")]
        [HttpPost]
        public async Task<ActionResult<Result<CreatedProductDto>>> CreateProductCommand(CreateProductCommand command)
        {
            return await _mediator.Send(command);
        }


        [Authorize("ProductUpdateRole")]
        [HttpPut]
        public async Task<ActionResult<Result<Product>>> UpdateProductCommand([FromBody] UpdateProductCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }



        [Authorize("ProductUpdateRole")]
        [HttpPut]
        [Route("GetStoreProduct")]
        public async Task<ActionResult<Result<Product>>> UpdateGetStoreProductCommand([FromBody] UpdateGetStoreProductCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }




        [Authorize("ProductUpdateRole")]
        [HttpPut]
        [Route("Transfer")]
        public async Task<ActionResult<Result<Product>>> UpdateProductOperationsCommand([FromBody] UpdateProductOperationsCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }



        [Authorize("ProductDeleteRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductCommand(int id)
        {
            await _mediator.Send(new DeleteProductCommand(id));

            return NoContent();
        }





        [Authorize("ProductReadRole")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetProductWithPaginationDto>>> GetCategoryWithPaginationQuery([FromQuery] GetProductWithPaginationQuery query)
        {
            return await _mediator.Send(query);
        }


        [Authorize("AssignedProductReadRole")]
        [HttpGet]
        [Route("GetByIdProductAndDetailsQuery")]
        public async Task<ActionResult<Result<List<GetByIdProductAndDetailsDto>>>> GetByIdProductAndDetailsQuery([FromQuery] GetByIdProductAndDetailsQuery query)
        {
            return await _mediator.Send(query);
        }




        //[Authorize("AssignedProductReadRole")]
        //[HttpGet]
        //[Route("ProductsAndAssignedProducts")]
        //public async Task<ActionResult<PaginatedResult<GetProductsPaginationDto>>> GetProductsPaginationQuery([FromQuery] GetProductsPaginationQuery query)
        //{
        //    return await _mediator.Send(query);
        //}




        [Authorize("ProductReadRole")]
        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<PaginatedResult<GetProductSearchWithPaginationDto>>> GetProductSearchQuery([FromQuery] GetProductSearchWithPaginationQuery dto)
        {
            return await _mediator.Send(dto);
        }




        [Authorize("FileTransferCreateRole")]
        [HttpPost]
        [Route("filemanagement")]
        public async Task<ActionResult<Result<CreatedProductMovementDto>>> CreateProductFileTransferCommand(FileTransferCommand command)
        {
            return await _mediator.Send(command);
        }
    }
}
