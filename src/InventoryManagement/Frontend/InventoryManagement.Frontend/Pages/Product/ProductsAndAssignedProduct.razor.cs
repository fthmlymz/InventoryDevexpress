using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Company;
using InventoryManagement.Frontend.DTOs.Product;
using InventoryManagement.Frontend.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using InventoryManagement.Frontend.Services.Authorization;

namespace InventoryManagement.Frontend.Pages.Product
{
    public partial class ProductsAndAssignedProduct : ComponentBase
    {
        #region Injection
        [Inject] public ApiService? _apiService { get; set; }
        [Inject] public NotificationService? _notificationService { get; set; }
        [Inject] public IAuthorizationService? _authorizationService { get; set; }
        [Inject] public CommunicationService? _communicationService { get; set; }
        [Inject] public NavigationManager? _navigationManager { get; set; }
        [Inject] public ContextMenuService? _contextMenuService { get; set; }
        #endregion




        private PaginatedResult<GetProductWithPaginationDto>? productAndAssignedModel;
        private Dictionary<int, GetProductWithPaginationDto> originalProductAndAssignedDataList = new Dictionary<int, GetProductWithPaginationDto>();
        private List<GetProductWithPaginationDto> assignedProductToUpdate = new List<GetProductWithPaginationDto>();
        IList<GetProductWithPaginationDto>? selectedProductAndAssignedProduct;





        #region Pagination
        private int pageNumber = 1;
        private int pageSize = 20;
        private RadzenDataGrid<GetProductWithPaginationDto>? productGrid;
        string pagingSummaryFormat = "Gösterilen sayfa {0} - {1} (toplam {2} kayıt)";
        IEnumerable<int> pageSizeOptions = new int[] { 5, 10, 20, 30, 50, 100, 500, 1000, 5000 };
        //List<int> pageSizeOptions = new List<int> { 5, 10, 20, 30, 50, 100, 500, 1000, 5000 };


        async void ChangePage(PagerEventArgs args)
        {
            pageNumber = args.PageIndex + 1;
            pageSize = productAndAssignedModel.pageSize;
            ProductFiltering(pageNumber, pageSize, filtreleme);
            StateHasChanged();
        }
        void PageSizeOptionsChanged(int pageSize)
        {
            productAndAssignedModel.pageSize = pageSize;
            ChangePage(new PagerEventArgs() { PageIndex = productGrid.CurrentPage });
        }
        #endregion




        protected override async Task OnInitializedAsync()
        {
            await LoadProductsAndAssignedProduct(pageNumber, pageSize);
        }

        private async Task LoadProductsAndAssignedProduct(int pageNumber, int pageSize)
        {
            try
            {
                productAndAssignedModel = await _apiService.GetAsync<PaginatedResult<GetProductWithPaginationDto>>($"{ApiEndpointConstants.ProductGetPostPutDelete}?PageNumber={pageNumber}&PageSize={pageSize}");
            }
            catch (Exception ex)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"Ürün listesi yüklenemedi : {ex.Message}", duration: 6000);
            }
            try
            {
                companies = await _apiService.GetAsync<PaginatedResult<CompanyAllModel>>($"{ApiEndpointConstants.CompanyGetAllList}");
            }
            catch (Exception ex)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"Şirket listesi alınamadı : {ex.Message}", duration: 6000);
            }
        }



        void OnCellContextMenu(DataGridCellMouseEventArgs<GetProductWithPaginationDto> args)
        {
            selectedProductAndAssignedProduct = new List<GetProductWithPaginationDto>() { args.Data };

            var contextMenuItems = new List<ContextMenuItem>();

            if (_authorizationService.HasPermission("res:assignedproduct", "scopes:create"))
            {
                contextMenuItems.Add(new ContextMenuItem() { Text = "Zimmetle", Value = 1, Icon = "person" });
                contextMenuItems.Add(new ContextMenuItem() { Text = "Teslim Al", Value = 2, Icon = "warehouse" });
                contextMenuItems.Add(new ContextMenuItem() { Text = "Ürünü Transfer Et", Value = 3, Icon = "local_shipping" });
            }
            if (_authorizationService.HasPermission("res:assignedproduct", "scopes:read"))
            {
                contextMenuItems.Add(new ContextMenuItem() { Text = "Ürün Detayları", Value = 4, Icon = "info" });
            }


            _contextMenuService.Open(args, contextMenuItems,
            async (e) =>
            {
                if (e.Value.ToString() == "1" && _authorizationService.HasPermission("res:assignedcreate", "scopes:create"))
                {
                    Console.WriteLine("Zimmetle");
                    //await ShowInlineDialog();
                }
                else if (e.Value.ToString() == "2")
                {
                    Console.WriteLine("Teslim al");
                }
                else if (e.Value.ToString() == "3")
                {
                    Console.WriteLine("transfer et");
                }
                else if (e.Value.ToString() == "4")
                {
                    await _communicationService.SendProductId(args.Data.Id);
                    await _communicationService.SendProductBarcode(Convert.ToInt32(args.Data.Barcode));
                    _navigationManager.NavigateTo("/product/product-and-assigned-details");
                }
            });
        }









        #region Filtering - Search
        private PaginatedResult<CompanyAllModel>? companies;
        private CompanyAllModel? selectedCompany;
        void OnDropDownValueChanged(object item)
        {
            if (item != null)
            {
                selectedCompany = (CompanyAllModel)item;
                filterValues[nameof(ProductModel.CompanyId)] = selectedCompany.Id.ToString();
                filtreleme.CompanyId = selectedCompany.Id;
            }
            else
            {
                selectedCompany = null;
                filterValues[nameof(ProductModel.CompanyId)] = string.Empty;
                filtreleme.CompanyId = null;
            }
            ProductFiltering();
        }




        RadzenDataFilter<GetProductWithPaginationDto>? dataFilter;
        IQueryable<GetProductWithPaginationDto>? filteredProducts;
        IQueryable<GetProductWithPaginationDto>? products;
        Dictionary<string, string> filterValues = typeof(GetProductWithPaginationDto).GetProperties().ToDictionary(p => p.Name, _ => string.Empty);
        private FilteringParameters filtreleme = new FilteringParameters();


        void ProductFiltering()
        {
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
            ProductFiltering(pageNumber, pageSize, filtreleme);
        }
        async void ProductFiltering(int pageNumber, int pageSize, FilteringParameters filtreleme)
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
            var url = $"{ApiEndpointConstants.ProductSearch}?{string.Join("&", filteredParameters)}";

            try
            {
                productAndAssignedModel = await _apiService.GetAsync<PaginatedResult<GetProductWithPaginationDto>>(url);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"API sunucusuna bağlanılamıyor {ex.Message}", duration: 6000);
            }

            StateHasChanged();
        }
        #endregion
    }
}
