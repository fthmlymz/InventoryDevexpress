using InventoryManagement.Application.Features.Brands.Commands.CreateBrand;
using InventoryManagement.Application.Features.Brands.Commands.DeleteBrand;
using InventoryManagement.Application.Features.Brands.Commands.UpdateBrand;
using InventoryManagement.Application.Features.Brands.Queries.GetBrandAllList;
using InventoryManagement.Application.Features.Brands.Queries.GetBrandListWithPaginationQuery;
using InventoryManagement.Application.Features.Brands.Queries.GetBrandWithModel;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [Authorize("BrandReadRole")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetBrandWithPaginationDto>>> GetBrandWithPaginationQuery([FromQuery] GetBrandWithPaginationQuery query)
        {
            return await _mediator.Send(query);
        }


        [Authorize("BrandReadRole")]
        [HttpGet]
        [Route("BrandWithModel")]
        public async Task<ActionResult<Result<GetBrandWithModelDto>>> GetBrandWithModelQuery(int companyId)
        {
            return await _mediator.Send(new GetBrandWithModelQuery(companyId));
        }



        [Authorize("BrandCreateRole")]
        [HttpPost]
        public async Task<ActionResult<Result<CreatedBrandDto>>> CreateBrandCommand(CreateBrandCommand command)
        {
            return await _mediator.Send(command);
        }



        [Authorize("BrandUpdateRole")]
        [HttpPut]
        public async Task<ActionResult<Result<Brand>>> UpdateBrandCommand([FromBody] UpdateBrandCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }


        [Authorize("BrandDeleteRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBrandCommand(int id)
        {
            await _mediator.Send(new DeleteBrandCommand(id));

            return NoContent();
        }

        [Authorize("BrandReadRole")]
        [HttpGet]
        [Route("BrandAllList")]
        public async Task<ActionResult<Result<List<GetBrandAllListDto>>>> GetBrandAllListQuery()
        {
            return await _mediator.Send(new GetBrandAllListQuery());
        }
    }
}
