using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.DTOs;

namespace InventoryManagement.Application.Features.TransferOfficiers.Queries.GetAllCompanyAndTransferOfficer
{

    public sealed record GetAllCompanyAndTransferOfficerQuery() : IRequest<Result<List<GetAllCompanyAndTransferOfficerDto>>>;




    internal class GetAllCompanyAndTransferOfficerQueryHandler : IRequestHandler<GetAllCompanyAndTransferOfficerQuery, Result<List<GetAllCompanyAndTransferOfficerDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCompanyAndTransferOfficerQueryHandler> _logger;

        public GetAllCompanyAndTransferOfficerQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCompanyAndTransferOfficerQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<List<GetAllCompanyAndTransferOfficerDto>>> Handle(GetAllCompanyAndTransferOfficerQuery request, CancellationToken cancellationToken)
        {
            var groupedData = await _unitOfWork.Repository<Company>()
                .Entities
                .Include(c => c.TransferOfficiers)
                .Where(c => c.TransferOfficiers.Any())
                .OrderBy(c => c.Name)
                .SelectMany(c => c.TransferOfficiers, (company, transferOfficier) => new GetAllCompanyAndTransferOfficerDto
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name,
                    UserName = transferOfficier.UserName,
                    FullName = transferOfficier.FullName,
                    Email = transferOfficier.Email,
                    Id = transferOfficier.Id,
                    CreatedBy = transferOfficier.CreatedBy,
                    CreatedDate = transferOfficier.CreatedDate,
                    CreatedUserId = transferOfficier.CreatedUserId,
                    UpdatedBy = transferOfficier.UpdatedBy,
                    UpdatedUserId = transferOfficier.UpdatedUserId,
                    UpdatedDate = transferOfficier.UpdatedDate,
                })
                .ToListAsync();
            return await Result<List<GetAllCompanyAndTransferOfficerDto>>.SuccessAsync(groupedData);
        }
    }
}
