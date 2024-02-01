using InventoryManagement.Application.Features.Reports.Queries.GetProductCountsQuery;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [Authorize("ReportReadRole")]
        [HttpGet]
        [Route("GetProductCountsQuery")]
        public async Task<ActionResult<Result<List<CategoryProductCountsDto>>>> GetProductCountsQuery()
        {
            return await _mediator.Send(new GetProductCountsQuery());
        }
    }
}
