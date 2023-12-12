using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.TransferOfficiers.Queries.GetByIdCompanyAndTransferOfficer
{
    public sealed record GetByIdCompanyAndTransferOfficerQuery(int CompanyId) : IRequest<Result<List<GetByIdCompanyAndTransferOfficerDto>>>;



    internal class GetByIdCompanyAndTransferOfficerQueryHandler : IRequestHandler<GetByIdCompanyAndTransferOfficerQuery, Result<List<GetByIdCompanyAndTransferOfficerDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetByIdCompanyAndTransferOfficerQueryHandler> _logger;

        public GetByIdCompanyAndTransferOfficerQueryHandler(IUnitOfWork unitOfWork, ILogger<GetByIdCompanyAndTransferOfficerQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<List<GetByIdCompanyAndTransferOfficerDto>>> Handle(GetByIdCompanyAndTransferOfficerQuery request, CancellationToken cancellationToken)
        {
            var transferOfficers = await _unitOfWork.Repository<TransferOfficier>()
                .Entities
                .Include(to => to.Company)
                .Where(to => to.CompanyId == request.CompanyId)
                .OrderByDescending(to => to.CreatedDate)
                .ToListAsync();

            var groupedData = transferOfficers
                .GroupBy(to => to.CompanyId)
                .Select(group => new GetByIdCompanyAndTransferOfficerDto
                {
                    CompanyId = group.Key,
                    Id = group.Key,
                    Name = group.FirstOrDefault()?.Company?.Name,
                    TransferOfficers = group.Select(to => new TransferOfficerQueryDto
                    {
                        Id = to.Id,
                        UserName = to.UserName,
                        FullName = to.FullName,
                        Email = to.Email
                    }).ToList()
                })
                .ToList();

            return await Result<List<GetByIdCompanyAndTransferOfficerDto>>.SuccessAsync(groupedData);
        }
    }
}
