using Application.Features.Companies.Commands.CreateCompany;
using Application.Features.Companies.Commands.UpdateCompany;
using Application.Features.Companies.Queries.CompanySearchWithPagination;
using Application.Features.Companies.Queries.GetCompanyAllList;
using Application.Features.Companies.Commands.DeleteCompany;
//using Features.Companies.Queries.GetCompanyAllList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase //ApiControllerBase
    {
        private readonly IMediator _mediator;

        public CompanyController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [Authorize("CompanyReadRole")]
        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<PaginatedResult<GetCompanySearchWithPaginationDto>>> GetCompanySearchQuery([FromQuery] GetCompanySearchWithPaginationQuery dto)
        {
            return await _mediator.Send(dto);
        }


        //Frontend - product/product-list for view
        [Authorize("ProductReadRole")]
        [HttpGet]
        [Route("companylist")]
        public async Task<ActionResult<Result<List<GetCompanyAllListDto>>>> GetCompanyList()
        {
            return await _mediator.Send(new GetCompanyAllListQuery());
        }




        [Authorize("CompanyCreateRole")]
        [HttpPost]
        public async Task<ActionResult<Result<CreatedCompanyDto>>> CreateCompanyCommand(CreateCompanyCommand command)
        {
            return await _mediator.Send(command);
        }


        [Authorize("CompanyDeleteRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<NoContent>>> DeleteCompanyCommand(int id)
        {
            await _mediator.Send(new DeleteCompanyCommand(id));
            return NoContent();
        }


        [Authorize("CompanyUpdateRole")]
        [HttpPut]
        public async Task<ActionResult<Result<NoContent>>> UpdateCompanyCommand([FromBody] UpdateCompanyCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }






    }
}


/*
  //Companies Query alanında GetCompanyListWithPagination silinecek
        //[HttpGet]
        //[Route("CompanyWithPagination")]
        //public async Task<ActionResult<PaginatedResult<GetCompanyListWithPaginationDto>>> GetCompanyListWithPaginationQuery([FromQuery] GetCompanyListWithPaginationQuery query)
        //{
        //    var validator = new GetCompanyListWithPaginationValidator();
        //    var result = validator.Validate(query);
        //    if (result.IsValid)
        //    {
        //        return await _mediator.Send(query);
        //    }
        //    var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
        //    return BadRequest(errorMessages);
        //}

        //[Authorize(Roles="CompanyReadRole",Policy = "company#read")]
        //[Authorize(Policy = "scopes:view")]
*/