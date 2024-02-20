using InventoryManagement.Application.Features.Models.Commands.CreateModel;
using InventoryManagement.Application.Features.Models.Commands.DeleteModel;
using InventoryManagement.Application.Features.Models.Commands.UpdateModel;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ModelController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [Authorize("ModelCreateRole")]
        [HttpPost]
        public async Task<ActionResult<Result<CreatedModelDto>>> CreateModelCommand(CreateModelCommand command)
        {
            return await _mediator.Send(command);
        }




        [Authorize("ModelUpdateRole")]
        [HttpPut]
        public async Task<ActionResult<Result<Model>>> UpdateModelCommand([FromBody] UpdateModelCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }



        [Authorize("ModelDeleteRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteModelCommand(int id)
        {
            await _mediator.Send(new DeleteModelCommand(id));

            return NoContent();
        }
    }
}
