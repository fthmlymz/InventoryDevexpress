using Application.Features.Categories.Commands.CreateCategory;
using Application.Features.Categories.Commands.UpdateCategory;
using Application.Features.Categories.Queries.GetCategoryListWithPaginationQuery;
using Domain.Entities;
using InventoryManagement.Application.Features.Categories.Commands.DeleteCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [Authorize("CategoryReadRole")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetCategoryWithPaginationDto>>> GetCategoryWithPaginationQuery([FromQuery] GetCategoryWithPaginationQuery query)
        {
            return await _mediator.Send(query);
        }

        [Authorize("CategoryCreateRole")]
        [HttpPost]
        public async Task<ActionResult<Result<CreatedCategoryDto>>> CreateCategoryCommand(CreateCategoryCommand command)
        {
            return await _mediator.Send(command);
        }


        [Authorize("CategoryUpdateRole")]
        [HttpPut]
        public async Task<ActionResult<Result<Category>>> UpdateCategoryCommand([FromBody] UpdateCategoryCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }



        [Authorize("CategoryDeleteRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryCommand(int id)
        {
            await _mediator.Send(new DeleteCategoryCommand(id));

            return NoContent();
        }
    }
}
