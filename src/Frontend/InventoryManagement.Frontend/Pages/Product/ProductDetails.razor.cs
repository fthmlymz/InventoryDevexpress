using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.UI;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Keycloak;
using InventoryManagement.Frontend.DTOs.Product;
using InventoryManagement.Frontend.DTOs.TransferOfficier;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using SharedLibrary.Common;

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
        [Inject] public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        #endregion


        public ProductDto? SelectedProduct { get; set; }
        public KeycloakUsersDto? SelectedUser { get; set; }
        public PaginatedResult<GetByIdProductAndDetailsDto>? productDetails { get; set; }
        public PaginatedResult<TransferOfficierDto>? productOfficierList { get; set; }
        private GetStoreProductDto? _storeProductDto { get; set; } = new GetStoreProductDto();
        private ApproveRejectDto? approveRejectDto { get; set; } = new ApproveRejectDto();

        public FileManager.FileManager? fileManagerRef;


        #region Dialogs
        private bool ShowUsersVisible { get; set; }
        private bool ShowGetStoreVisible { get; set; }
        private bool ShowTransferListVisible { get; set; }
        private bool ShowAssignedFormVisible { get; set; }
        #endregion


        #region Report
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        DxReportViewer _reportViewer;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        XtraReport _report = XtraReport.FromFile(ApplicationConstants.ZimmetFormuReport, true);
        #endregion
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
            }
            catch (Exception ex)
            {
                NotificationService?.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"İstek gerçekleştirilemedi {ex.Message}", duration: 6000);
            }
            SelectedProduct!.Status = productDetails?.data?.FirstOrDefault()?.Status;
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }



        #region Get Store Product
        async Task ProductGetStore()
        {
            _storeProductDto!.Id = SelectedProduct!.ProductId!.Value;

            var result = await ApiService!.PutAsync(ApiEndpointConstants.GetStoreProduct, _storeProductDto);
            if (result.IsSuccessStatusCode)
            {
                NotificationService?.Notify(NotificationSeverity.Success, "Depo İşlemi", $"{SelectedProduct.Barcode} barkod numaralı ürün depoya alındı.", duration: 6000);
                await ProductGetDetails();
            }

            await fileManagerRef!.SearchFiles();
            await InvokeAsync(StateHasChanged);
            /*System.NullReferenceException: 'Object reference not set to an instance of an object.'

fileManagerRef was null.
            */
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
                ProductName = SelectedProduct?.Name,
                Company = SelectedUser?.Attributes?.Company?.FirstOrDefault(),
                Department = SelectedUser?.Attributes?.Department?.FirstOrDefault(),
                Manager = SelectedUser?.Attributes?.Manager?.FirstOrDefault(),
                PhysicalDeliveryOfficeName = SelectedUser?.Attributes?.PhysicalDeliveryOfficeName?.FirstOrDefault(),
                Title = SelectedUser?.Attributes?.Title?.FirstOrDefault(),
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


        #region Assigned Product Form
        void ReportSet()
        {
            #region Report
            _report = XtraReport.FromFile(ApplicationConstants.ZimmetFormuReport, true);
            _report.DataSource = productDetails;
            _report.DataMember = "data";
            #endregion

        }
        #endregion


        #region Product Approve Reject
        async Task ProductApproveReject(string status)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            approveRejectDto!.ProductId = SelectedProduct.ProductId;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            approveRejectDto.AssignedProductId = productDetails?.data?.FirstOrDefault()?.AssignedProducts?.FirstOrDefault()?.Id;
            approveRejectDto.ApprovalStatus = status;


            var result = await ApiService!.PutAsync(ApiEndpointConstants.AssignedProductApproveReject, approveRejectDto);

            if (result.IsSuccessStatusCode)
            {
                var severity = status == "Accepted" ? NotificationSeverity.Success : NotificationSeverity.Error;
                var message = status == "Accepted" ? "onayladınız" : "reddettiniz";
                var fullName = productDetails?.data?.FirstOrDefault()?.AssignedProducts?.FirstOrDefault()?.FullName;
                var barcode = productDetails?.data?.FirstOrDefault()?.Barcode;

                if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(barcode.ToString()))
                {
                    NotificationService?.Notify(severity, status, $"Sayın {fullName}, {barcode} numaralı ürünü {message}.", duration: 6000);
                }

                await ProductGetDetails();
            }
        }
        #endregion


        #region Product Transfer
        async Task ProductTransferGetList()
        {
            productOfficierList = await ApiService!.GetAsync<PaginatedResult<TransferOfficierDto>>($"{ApiEndpointConstants.GetTransferOfficierGetAll}");
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        async Task ProductTransfer(TransferOfficierDto data)
        {

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ProductTransferDto dto = new ProductTransferDto
            {
                Id = SelectedProduct!.ProductId!.Value,
                SenderUserName = ((KeycloakAuthenticationStateProvider)AuthenticationStateProvider).UserInfo.PreferredUsername,
                SenderEmail = ((KeycloakAuthenticationStateProvider)AuthenticationStateProvider).UserInfo.Email,
                SenderCompanyName = SelectedProduct.CompanyName,

                RecipientCompanyId = data.CompanyId,
                RecipientEmail = data.Email,
                RecipientUserName = data.UserName,

                TypeOfOperations = GenericConstantDefinitions.Transfer
            };
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var result = await ApiService!.PutAsync(ApiEndpointConstants.PutProductTransfer, dto);
            if (result.IsSuccessStatusCode)
            {
                NotificationService?.Notify(NotificationSeverity.Success, "Transfer", $"{SelectedProduct.Barcode} barkodlu ürün transfer işlemi başladı.", duration: 6000);
            }
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });


            await ProductGetDetails();
        }
        async Task ProductTransfer(string transferValue)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string typeOfOperation = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            switch (transferValue)
            {
                case GenericConstantDefinitions.Accepted:
                    typeOfOperation = GenericConstantDefinitions.Accepted;
                    break;
                case GenericConstantDefinitions.Rejected:
                    typeOfOperation = GenericConstantDefinitions.Rejected;
                    break;
                case GenericConstantDefinitions.ReturnIt:
                    typeOfOperation = GenericConstantDefinitions.ReturnIt;
                    break;
            }
            if (typeOfOperation == null)
            {
                return;
            }

            ProductTransferDto dto = new ProductTransferDto
            {
                Id = SelectedProduct!.ProductId!.Value,
                TypeOfOperations = typeOfOperation,
            };
            var result = await ApiService!.PutAsync(ApiEndpointConstants.PutProductTransfer, dto);

            if (result.IsSuccessStatusCode)
            {
                NotificationService?.Notify(NotificationSeverity.Success, "Transfer", $"{SelectedProduct.Barcode} barkodlu ürün için {transferValue} işlemi yapıldı", duration: 6000);
            }

            await ProductGetDetails();
        }
        #endregion
    }
}
