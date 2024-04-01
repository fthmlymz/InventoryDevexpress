using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Linq.Expressions;

namespace Application.Features.Companies.Queries.CompanySearchWithPagination
{
    //Best performance
    public class GetCompanySearchWithPaginationQuery : IRequest<PaginatedResult<GetCompanySearchWithPaginationDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public GetCompanySearchWithPaginationQuery() { }
        public GetCompanySearchWithPaginationQuery(int pageNumber, int pageSize, string name, string description, string createdBy, string updatedBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Name = name;
            Description = description;
            CreatedBy = createdBy;
            UpdatedBy = updatedBy;
        }
    }
    internal class GetCompanySearchQueryHandler : IRequestHandler<GetCompanySearchWithPaginationQuery, PaginatedResult<GetCompanySearchWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEasyCacheService _cacheService;

        public GetCompanySearchQueryHandler(IUnitOfWork unitOfWork, IEasyCacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }


        public async Task<PaginatedResult<GetCompanySearchWithPaginationDto>> Handle(GetCompanySearchWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = GetCacheKey(request);
            var cacheResult = await _cacheService.GetAsync<PaginatedResult<GetCompanySearchWithPaginationDto>>(cacheKey);
            if (cacheResult != null)
            {
                return cacheResult;
            }
            var result = await SearchCompanies(request);
            await _cacheService.SetAsync(cacheKey, result);
            return result;
        }

        private string GetCacheKey(GetCompanySearchWithPaginationQuery request)
        {
            var cacheKey = "CompanySearch_";
            foreach (var property in typeof(GetCompanySearchWithPaginationQuery).GetProperties())
            {
                var value = property.GetValue(request);
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    cacheKey += value.ToString() + "_";
                }
            }
            cacheKey = cacheKey.TrimEnd('_');
            return cacheKey;
        }

        private async Task<PaginatedResult<GetCompanySearchWithPaginationDto>> SearchCompanies(GetCompanySearchWithPaginationQuery request)
        {
            var searchPredicate = BuildSearchPredicate(request);

            var totalCount = await _unitOfWork.Repository<Company>()
                .Where(searchPredicate)
                .CountAsync();

            var result = await _unitOfWork.Repository<Company>()
                .Where(searchPredicate)
                .OrderBy(p => p.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new GetCompanySearchWithPaginationDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    UpdatedDate = p.UpdatedDate,
                    CreatedDate = p.CreatedDate,
                    CreatedBy = p.CreatedBy,
                    UpdatedBy = p.UpdatedBy
                })
                .ToListAsync();
            return PaginatedResult<GetCompanySearchWithPaginationDto>.Create(result, totalCount, request.PageNumber, request.PageSize);
        }
        private Expression<Func<Company, bool>> BuildSearchPredicate(GetCompanySearchWithPaginationQuery request)
        {
            var parameter = Expression.Parameter(typeof(Company), "p");
            Expression body = Expression.Constant(false);
            bool isAnySearchFieldProvided = false; // Arama alanlarından herhangi biri sağlandı mı?
            foreach (var property in typeof(GetCompanySearchWithPaginationQuery).GetProperties())
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(request);

                if (propertyValue != null && property.PropertyType == typeof(string))
                {
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var propertyExpression = Expression.Property(parameter, propertyName);
                    var valueExpression = Expression.Constant(propertyValue);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
#pragma warning disable CS8604 // Possible null reference argument.
                    var propertyToLowerExpression = Expression.Call(propertyExpression, toLowerMethod);
#pragma warning restore CS8604 // Possible null reference argument.
                    var valueToLowerExpression = Expression.Call(valueExpression, toLowerMethod);
#pragma warning disable CS8604 // Possible null reference argument.
                    var containsExpression = Expression.Call(propertyToLowerExpression, containsMethod, valueToLowerExpression);
#pragma warning restore CS8604 // Possible null reference argument.
                    body = Expression.OrElse(body, containsExpression);
                    isAnySearchFieldProvided = true;
                }
            }
            if (!isAnySearchFieldProvided)
            {
                return Expression.Lambda<Func<Company, bool>>(Expression.Constant(true), parameter);
            }
            return Expression.Lambda<Func<Company, bool>>(body, parameter);
        }
    }
}
