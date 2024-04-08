using Application.Features.Reports.Queries.GeneralReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Web.API.Controllers
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
        [Route("GeneralReport")]
        public async Task<ActionResult<Result<List<CombinedProductCountsDto>>>> ProductCountsAllQuery2()
        {
            return await _mediator.Send(new CombinedProductCountsQuery());
        }
    }
}
