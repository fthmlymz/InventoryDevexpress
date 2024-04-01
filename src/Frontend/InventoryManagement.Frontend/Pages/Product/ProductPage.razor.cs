using DevExpress.Blazor;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Product;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace InventoryManagement.Frontend.Pages.Product
{
    public partial class ProductPage : ComponentBase
    {
        #region Inject
        [Inject] ApiService? ApiService { get; set; }
        [Inject] IAuthorizationService? AuthorizationService { get; set; }
        [Inject] NotificationService? NotificationService { get; set; }
        [Inject] public CommunicationService? CommunicationService { get; set; }
        [Inject] public NavigationManager? NavigationManager { get; set; }

        #endregion

        private IGrid? GridProduct { get; set; }


        #region Pagination
        private PaginatedResult<ProductDto>? productModel { get; set; }
        private int PageNumber { get; set; } = 1;
        int PageSize { get; set; } = 250;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            await GetProductList();
        }

        async Task GetProductList()
        {
            productModel = await ApiService!.GetAsync<PaginatedResult<ProductDto>>($"{ApiEndpointConstants.GetProduct}?PageNumber={PageNumber}&PageSize={PageSize}");
            StateHasChanged();
        }

        async Task HandleProductPageNumberChanged(int newPageNumber)
        {
            PageNumber = newPageNumber + 1;
            await GetProductList();
            StateHasChanged();
        }


        #region Product Filtering
        private async Task OnFilterValueChanged(string columnName, string filterValue)
        {
            filterValues[columnName] = filterValue;
            await ProductFiltering();
        }
        Dictionary<string, string> filterValues = typeof(ProductDto).GetProperties().ToDictionary(p => p.Name, _ => string.Empty);
        async Task ProductFiltering()
        {
            var filtreleme = new FilteringParameters();
            foreach (var property in typeof(FilteringParameters).GetProperties())
            {
                if (filterValues.ContainsKey(property.Name))
                {
                    if (property.PropertyType == typeof(int?))
                    {
                        int? value = null;
                        if (int.TryParse(filterValues[property.Name], out int parsedValue))
                        {
                            value = parsedValue;
                        }
                        property.SetValue(filtreleme, value);
                    }
                    else
                    {
                        property.SetValue(filtreleme, filterValues[property.Name]);
                    }
                }
            }
            await ProductFiltering(PageNumber, PageSize, filtreleme);
        }
        async Task ProductFiltering(int pageNumber, int pageSize, FilteringParameters filtreleme)
        {
            var parameters = new Dictionary<string, string>
            {
                { "PageNumber", pageNumber.ToString() }, { "PageSize", pageSize.ToString() }
            };

            var propertyInfos = filtreleme.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var value = propertyInfo.GetValue(filtreleme)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    parameters.Add(propertyInfo.Name, value);
                }
            }
            var filteredParameters = parameters.Where(p => p.Value != null).Select(p => $"{p.Key}={p.Value}");
            var url = $"{ApiEndpointConstants.SearchProduct}?{string.Join("&", filteredParameters)}";

            try
            {
                productModel = await ApiService!.GetAsync<PaginatedResult<ProductDto>>(url);
            }
            catch (Exception ex)
            {
                NotificationService?.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"API sunucusuna bağlanılamıyor {ex.Message}", duration: 6000);
            }

            StateHasChanged();
        }
        #endregion


        async Task GetProductDetails(object values)
        {
            await CommunicationService!.SendProduct((ProductDto)values);
            NavigationManager?.NavigateTo("/product/product-details");
        }
    }
}
