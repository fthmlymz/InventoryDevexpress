using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Keycloak;
using InventoryManagement.Frontend.DTOs.Product;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace InventoryManagement.Frontend.Pages.Product
{
    public partial class ProductDetails : ComponentBase
    {
        #region Inject
        [Inject] public CommunicationService? CommunicationService { get; set; }
        [Inject] public NavigationManager? NavigationManager { get; set; }
        [Inject] public ApiService? ApiService { get; set; }
        [Inject] public NotificationService? NotificationService { get; set; }
        [Inject] public IAuthorizationService? AuthorizationService { get; set; }
        #endregion


        public ProductDto? SelectedProduct { get; set; }
        public KeycloakUsersDto? SelectedUser { get; set; }
        public PaginatedResult<GetByIdProductAndDetailsDto>? productDetails { get; set; }
        private GetStoreProductDto? _storeProductDto { get; set; } = new GetStoreProductDto();



        private bool ShowUsersVisible { get; set; }
        private bool ShowGetStoreVisible { get; set; }



        protected override async void OnInitialized()
        {
            SelectedProduct = CommunicationService!.GetSelectedProduct();
            CommunicationService!.OnProductSelected += SetSelectedProduct;

            if (SelectedProduct == null)
            {
                NavigationManager?.NavigateTo("/product/product-list");
                return;
            }
            await ProductGetDetails();
        }
        private async Task SetSelectedProduct(ProductDto selectedProduct)
        {
            SelectedProduct = selectedProduct;
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }



        async Task ProductGetDetails()
        {
            try
            {
                productDetails = await ApiService!.GetAsync<PaginatedResult<GetByIdProductAndDetailsDto>>($"{ApiEndpointConstants.GetByIdProductAndDetailsQuery}?Id={SelectedProduct?.ProductId}");

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch (Exception ex)
            {
                NotificationService?.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"İstek gerçekleştirilemedi {ex.Message}", duration: 6000);
            }
            SelectedProduct!.Status = productDetails?.data?.FirstOrDefault()?.Status;
        }




        #region Get Store Product
        async Task ProductGetStore()
        {
            _storeProductDto!.Id = SelectedProduct!.ProductId!.Value;

            var result = await ApiService!.PutAsync(ApiEndpointConstants.GetStoreProduct, _storeProductDto);
            if (result.IsSuccessStatusCode)
            {
                await ProductGetDetails();
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        #endregion



        #region Assigned Product
        async Task AssignedProduct()
        {
            SelectedUser = CommunicationService!.GetSelectedUser();

            if (productDetails?.data == null)
            {
                return;
            }

            AssignedProductDto data = new AssignedProductDto
            {
                Id = Convert.ToInt32(productDetails?.data?.FirstOrDefault()?.AssignedProducts?.FirstOrDefault()?.Id),
                AssignedUserId = SelectedUser?.Attributes?.LDAP_ID?.FirstOrDefault(),
                AssignedUserName = SelectedUser?.Attributes?.sAMAccountName?.FirstOrDefault(),
                Barcode = SelectedProduct?.Barcode,
                Email = SelectedUser?.Email,
                FullName = SelectedUser?.FirstName + " " + SelectedUser?.LastName,
                ProductId = Convert.ToInt32(SelectedProduct?.ProductId),
                ProductName = SelectedProduct?.Name
            };

            var result = (productDetails?.data?.FirstOrDefault()?.AssignedProducts?.FirstOrDefault()?.Id == null)
                            ? await ApiService!.PostAsync(ApiEndpointConstants.AssignedProductCreate, data)
                            : await ApiService!.PutAsync(ApiEndpointConstants.AssignedProductUpdate, data);

            if (result.StatusCode == System.Net.HttpStatusCode.NoContent || result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                NotificationService?.Notify(NotificationSeverity.Success, "Zimmet", $"{SelectedUser?.FirstName} {SelectedUser?.LastName} kullanıcısına {SelectedProduct?.Barcode} numaralı ürün zimmetlendi.", duration: 6000);
            }

            await ProductGetDetails();
        }
        #endregion
    }
}
