using Application.Features.TransferOfficiers.Commands.CreateTransferOfficier;
using Application.Features.TransferOfficiers.Commands.UpdateTransferOfficier;
using Application.Features.TransferOfficiers.Queries.GetByIdCompanyAndTransferOfficer;
using InventoryManagement.Application.Features.TransferOfficiers.Commands.DeleteTransferOfficier;
using InventoryManagement.Application.Features.TransferOfficiers.Queries.GetAllCompanyAndTransferOfficer;
using InventoryManagement.Application.Features.TransferOfficiers.Queries.GetByIdCompanyAndTransferOfficer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared;
using SharedLibrary.DTOs;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferOfficierController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TransferOfficierController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [Authorize("TransferOfficierCreateRole")]
        [HttpPost]
        public async Task<ActionResult<Result<TransferOfficierDto>>> CreateTranferOfficierCommand(CreateTranferOfficierCommand command)
        {
            return await _mediator.Send(command);
        }



        [Authorize("TransferOfficierUpdateRole")]
        [HttpPut]
        public async Task<ActionResult<Result<NoContent>>> UpdateTranferOfficierCommand([FromBody] UpdateTransferOfficierCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }



        [Authorize("TransferOfficierDeleteRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransferOfficierCommand(int id)
        {
            await _mediator.Send(new DeleteTransferOfficierCommand(id));

            return NoContent();
        }



        [Authorize("TransferOfficierReadRole")]
        [HttpGet("{companyId}")]
        public async Task<ActionResult<Result<List<GetByIdCompanyAndTransferOfficerDto>>>> GetByIdCompanyAndTransferOfficerQuery(int companyId)
        {
            return await _mediator.Send(new GetByIdCompanyAndTransferOfficerQuery(companyId));
        }



        [Authorize("TransferOfficierReadRole")]
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<Result<List<GetAllCompanyAndTransferOfficerDto>>>> GetAllCompanyAndTransferOfficerQuery()
        {
            return await _mediator.Send(new GetAllCompanyAndTransferOfficerQuery());
        }
    }
}
