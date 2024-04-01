using Application.Features.AssignedProducts.Commands.AcceptRejectProduct;
using Application.Features.AssignedProducts.Commands.CreateAssignedProduct;
using Application.Features.AssignedProducts.Commands.UpdateAssignedProduct;
using Application.Features.AssignedProducts.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignedProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssignedProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize("AssignedProductCreateRole")]
        [HttpPost]
        [Route("AssignedProductCreate")]
        public async Task<ActionResult<Result<AssignedProductDto>>> CreateAssignedProductCommand(CreateAssignedProductCommand command)
        {
            return await _mediator.Send(command);
        }




        [Authorize("AssignedProductUpdateRole")]
        [HttpPut]
        [Route("AssignedProductUpdate")]
        public async Task<ActionResult<Result<AssignedProductDto>>> UpdateAssignedProductCommand([FromBody] UpdateAssignedProductCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }



        [Authorize("AssignedProductUpdateRole")]
        [HttpPut]
        [Route("AssignedProductApproveReject")]
        public async Task<ActionResult<Result<NoContent>>> UpdatedAcceptRejectProductCommand([FromBody] UpdatedAcceptRejectProductCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }


    }
}
