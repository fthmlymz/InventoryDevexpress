﻿using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryManagement.Application.Features.Products.Queries.ProductSearchWithPagination
{
    public class GetProductSearchWithPaginationQuery : IRequest<PaginatedResult<GetProductSearchWithPaginationDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Name { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Barcode { get; set; }
        public string? SerialNumber { get; set; }
        public string? Imei { get; set; }
        public string? Mac { get; set; }
        public string? Status { get; set; }
        public int? CompanyId { get; set; }
        public GetProductSearchWithPaginationQuery() { }
        public GetProductSearchWithPaginationQuery(int pageNumber, int pageSize, string name, string createdBy, string updatedBy, string barcode, string serialNumber,
            string status, string? imei, string? mac, int? companyId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Name = name;
            CreatedBy = createdBy;
            UpdatedBy = updatedBy;
            Barcode = barcode;
            SerialNumber = serialNumber;
            Imei = imei;
            Status = status;
            Mac = mac;
            CompanyId = companyId;
        }
    }

    internal class GetProductSearchQueryHandler : IRequestHandler<GetProductSearchWithPaginationQuery, PaginatedResult<GetProductSearchWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEasyCacheService _cacheService;

        public GetProductSearchQueryHandler(IUnitOfWork unitOfWork, IEasyCacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<PaginatedResult<GetProductSearchWithPaginationDto>> Handle(GetProductSearchWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = GetCacheKey(request);
            var cacheResult = await _cacheService.GetAsync<PaginatedResult<GetProductSearchWithPaginationDto>>(cacheKey);
            if (cacheResult != null)
            {
                return cacheResult;
            }
            var result = await SearchProducts(request);
            await _cacheService.SetAsync(cacheKey, result);
            return result;
        }
        private string GetCacheKey(GetProductSearchWithPaginationQuery request)
        {
            var cacheKey = "ProductSearch_";
            foreach (var property in typeof(GetProductSearchWithPaginationQuery).GetProperties())
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

        private async Task<PaginatedResult<GetProductSearchWithPaginationDto>> SearchProducts(GetProductSearchWithPaginationQuery request)
        {
            var searchPredicate = BuildSearchPredicate(request);

            var totalCount = await _unitOfWork.Repository<Product>()
            .Where(searchPredicate)
            .CountAsync();


            var result = await _unitOfWork.Repository<Product>()
              .Where(searchPredicate)
              .OrderBy(p => p.Name)
              .Skip((request.PageNumber - 1) * request.PageSize)
              .Take(request.PageSize)
              .Select(p => new GetProductSearchWithPaginationDto
              {
                  Id = p.Id,
                  Name = p.Name,
                  Barcode = p.Barcode,
                  CompanyId = p.CompanyId,
                  CreatedBy = p.CreatedBy,
                  CreatedDate = p.CreatedDate,
                  DataClass = p.DataClass,
                  Imei = p.Imei,
                  InvoiceDate = p.InvoiceDate,
                  Mac = p.Mac,
                  ProductDate = p.ProductDate,
                  SerialNumber = p.SerialNumber,
                  Status = p.Status,
                  PurchaseDate = p.PurchaseDate,

                  UpdatedBy = p.UpdatedBy,
                  UpdatedDate = p.UpdatedDate,

                  BrandId = p.BrandId,
                  CategoryId = p.CategoryId,
                  CategorySubId = p.CategorySubId,
                  ModelId = p.ModelId,

                  CategoryName = p.Category != null ? p.Category.Name : null,
                  CategorySubName = p.CategorySub != null ? p.CategorySub.Name : null,
                  BrandName = p.Brand != null ? p.Brand.Name : null,
                  ModelName = p.Model != null ? p.Model.Name : null,

                  CompanyName = p.Company != null ? p.Company.Name : null,

              })
              .ToListAsync();
            return PaginatedResult<GetProductSearchWithPaginationDto>.Create(result, totalCount, request.PageNumber, request.PageSize);
        }

        private Expression<Func<Product, bool>> BuildSearchPredicate(GetProductSearchWithPaginationQuery request)
        {
            var parameter = Expression.Parameter(typeof(Product), "p");
            Expression body = Expression.Constant(true);
            bool isAnySearchFieldProvided = false; // Arama alanlarından herhangi biri sağlandı mı?

            // companyId için özel olarak ifade oluşturma
            if (request.CompanyId.HasValue)
            {
                var companyIdExpression = Expression.Property(parameter, nameof(Product.CompanyId));
                var companyIdValue = Expression.Constant(request.CompanyId.Value);
                var companyIdComparison = Expression.Equal(companyIdExpression, companyIdValue);
                body = Expression.AndAlso(body, companyIdComparison);
                isAnySearchFieldProvided = true;
            }

            foreach (var property in typeof(GetProductSearchWithPaginationQuery).GetProperties())
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(request);

                if (propertyValue != null && property.PropertyType == typeof(string) && propertyName != nameof(GetProductSearchWithPaginationQuery.CompanyId))
                {
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var propertyExpression = Expression.Property(parameter, propertyName);
                    var valueExpression = Expression.Constant(propertyValue);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var propertyToLowerExpression = Expression.Call(propertyExpression, toLowerMethod);
                    var valueToLowerExpression = Expression.Call(valueExpression, toLowerMethod);
                    var containsExpression = Expression.Call(propertyToLowerExpression, containsMethod, valueToLowerExpression);
                    body = Expression.AndAlso(body, containsExpression);
                    isAnySearchFieldProvided = true;
                }
            }
            if (!isAnySearchFieldProvided)
            {
                return Expression.Lambda<Func<Product, bool>>(Expression.Constant(true), parameter);
            }

            return Expression.Lambda<Func<Product, bool>>(body, parameter);
        }
    }
}




/*private Expression<Func<Product, bool>> BuildSearchPredicate(GetProductSearchWithPaginationQuery request)
{
    var parameter = Expression.Parameter(typeof(Product), "p");
    Expression body = Expression.Constant(false);
    bool isAnySearchFieldProvided = false; // Arama alanlarından herhangi biri sağlandı mı?
    foreach (var property in typeof(GetProductSearchWithPaginationQuery).GetProperties())
    {
        var propertyName = property.Name;
        var propertyValue = property.GetValue(request);

        if (propertyValue != null && property.PropertyType == typeof(string))
        {
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var propertyExpression = Expression.Property(parameter, propertyName);
            var valueExpression = Expression.Constant(propertyValue);
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var propertyToLowerExpression = Expression.Call(propertyExpression, toLowerMethod);
            var valueToLowerExpression = Expression.Call(valueExpression, toLowerMethod);
            var containsExpression = Expression.Call(propertyToLowerExpression, containsMethod, valueToLowerExpression);
            body = Expression.OrElse(body, containsExpression);
            isAnySearchFieldProvided = true;
        }
    }
    if (!isAnySearchFieldProvided)
    {
        return Expression.Lambda<Func<Product, bool>>(Expression.Constant(true), parameter);
    }
    return Expression.Lambda<Func<Product, bool>>(body, parameter);
}*/