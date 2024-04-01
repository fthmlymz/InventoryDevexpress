using DevExpress.Blazor;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Brand;
using InventoryManagement.Frontend.DTOs.Category;
using InventoryManagement.Frontend.DTOs.Company;
using InventoryManagement.Frontend.DTOs.Product;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;
using SharedLibrary.Common;

namespace InventoryManagement.Frontend.Pages.Definitions
{
    public partial class ProductEditorPage : ComponentBase
    {
        #region Inject
        [Inject] ApiService? ApiService { get; set; }
        [Inject] IAuthorizationService? AuthorizationService { get; set; }
        [Inject] NotificationService? NotificationService { get; set; }
        [Inject] DialogService? DialogService { get; set; }
        #endregion

        private IGrid? GridProduct { get; set; }

        #region Pagination
        private PaginatedResult<ProductDto>? productModel { get; set; }
        private int PageNumber { get; set; } = 1;
        int PageSize { get; set; } = 250;
        #endregion

        #region Definitions
        private PaginatedResult<CompanyDto>? companyDto { get; set; }
        private PaginatedResult<CategoryDto>? categoryDto { get; set; }
        private PaginatedResult<BrandDto>? brandDto { get; set; }
        #endregion


        #region Category, categorySub, Brand, Model
        IEnumerable<CompanyDto>? Companies { get; set; }
        IEnumerable<CategoryDto>? Categories { get; set; }
        IEnumerable<CategorySubDto>? CategorySubs { get; set; }
        IEnumerable<BrandDto>? Brands { get; set; }
        IEnumerable<ModelDto>? Models { get; set; }
        #endregion


        private IEnumerable<string> DataClassOptions { get; set; } = GenericConstantDataClassDescriptions.DescriptionArray;





        protected override async Task OnInitializedAsync()
        {
            await Task.WhenAll(
                GetProductList(),
                GetCompanies(),
                GetCategories(),
                GetBrands()
                );
        }

        #region Get All Definition
        private async Task GetCompanies()
        {
            companyDto = await ApiService!.GetAsync<PaginatedResult<CompanyDto>>($"{ApiEndpointConstants.AllCompanyList}");
            Companies = companyDto?.data;
            StateHasChanged();
        }
        private async Task GetCategories()
        {
            categoryDto = await ApiService!.GetAsync<PaginatedResult<CategoryDto>>($"{ApiEndpointConstants.GetCategory}?pageNumber=1&pageSize=100000");
            Categories = categoryDto?.data;
            if (Categories != null)
            {
                CategorySubs = Categories.SelectMany(country => country.CategorySubs ?? Enumerable.Empty<CategorySubDto>());
            }
            StateHasChanged();
        }
        private async Task GetBrands()
        {
            brandDto = await ApiService!.GetAsync<PaginatedResult<BrandDto>>($"{ApiEndpointConstants.GetBrand}?pageNumber=1&pageSize=100000");
            Brands = brandDto?.data;
            if (Brands != null)
            {
                Models = Brands.SelectMany(model => model.Models ?? Enumerable.Empty<ModelDto>());
            }

            StateHasChanged();
        }
        async Task GetProductList()
        {
            productModel = await ApiService!.GetAsync<PaginatedResult<ProductDto>>($"{ApiEndpointConstants.GetProduct}?PageNumber={PageNumber}&PageSize={PageSize}");
            StateHasChanged();
        }
        #endregion

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





        void GridProduct_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var product = (ProductDto)e.EditModel;
                product.Name = null;
            }
        }
        async Task GridProduct_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                var newProduct = (ProductDto)e.EditModel;
                newProduct.Status = GenericConstantDefinitions.InStock; // First record in stock

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var response = await ApiService.PostAsync(ApiEndpointConstants.PostProduct, newProduct);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (response.IsSuccessStatusCode)
                {
                    var insertedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();

#pragma warning disable CS8604 // Possible null reference argument.
                    FillCategoryNames(insertedProduct);
#pragma warning restore CS8604 // Possible null reference argument.

                    productModel?.data?.Add(insertedProduct);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    productModel.totalCount++;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    StateHasChanged();
                }
            }
            else
            {
                var updatedProduct = (ProductDto)e.EditModel;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var response = await ApiService.PutAsync($"{ApiEndpointConstants.PutProduct}", updatedProduct);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (response.IsSuccessStatusCode)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var index = productModel.data.FindIndex(c => c.Id == updatedProduct.ProductId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    if (index != -1)
                    {
                        productModel.data[index] = updatedProduct;
                        StateHasChanged();
                    }
                }
                GridProduct?.Reload();
                StateHasChanged();
            }
            StateHasChanged();
        }
        void FillCategoryNames(ProductDto product)
        {
            product.CategoryName = Categories?.FirstOrDefault(c => c.Id == product.CategoryId)?.Name;
            product.CategorySubName = CategorySubs?.FirstOrDefault(s => s.Id == product.CategorySubId)?.Name;
            product.CompanyName = Companies?.FirstOrDefault(c => c.Id == product.CompanyId)?.Name;
            product.BrandName = Brands?.FirstOrDefault(b => b.Id == product.BrandId)?.Name;
            product.ModelName = Models?.FirstOrDefault(m => m.Id == product.ModelId)?.Name;
        }


        async Task DeleteProduct(ProductDto product)
        {
            var response = await ApiService!.DeleteAsync(ApiEndpointConstants.DeleteProduct, product.Id);
            if (response.IsSuccessStatusCode)
            {
                productModel?.data?.Remove(product);
                productModel!.totalCount--;

                StateHasChanged();
                GridProduct?.Reload();
                NotificationService?.Notify(NotificationSeverity.Success, "Başarılı", $"{product.Name} isimli ürün ve bağlı alt kayıtlar silindi.");
            }
            else
            {
                NotificationService?.Notify(NotificationSeverity.Error, "Başarısız", $"{product.Name} silme işlemi gerçekleşti.");
            }
        }
    }
}
