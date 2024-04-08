using Application.Features.CategoriesSub.Commands.CreateCategorySub;
using Application.Features.CategoriesSub.Commands.UpdateCategorySub;
using Domain.Entities;
using Application.Features.CategoriesSub.Commands.DeleteCategorySub;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorySubController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategorySubController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [Authorize("CategorySubCreateRole")]
        [HttpPost]
        public async Task<ActionResult<Result<CreatedCategorySubDto>>> CreateCategorySubCommand(CreateCategorySubCommand command)
        {
            return await _mediator.Send(command);
        }


        [Authorize("CategorySubUpdateRole")]
        [HttpPut]
        public async Task<ActionResult<Result<CategorySub>>> UpdateCategorySubCommand([FromBody] UpdateCategorySubCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }


        [Authorize("CategorySubDeleteRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategorySubCommand(int id)
        {
            await _mediator.Send(new DeleteCategorySubCommand(id));

            return NoContent();
        }
    }
}
