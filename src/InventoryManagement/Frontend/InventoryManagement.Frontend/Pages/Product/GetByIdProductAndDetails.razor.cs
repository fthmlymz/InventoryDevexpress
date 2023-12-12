using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.UI;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Keycloak;
using InventoryManagement.Frontend.DTOs.Product;
using InventoryManagement.Frontend.DTOs.Products;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Radzen.Blazor;
using SharedLibrary.Common;
using SharedLibrary.DTOs;
using System.Net;
using System.Text.Json;

namespace InventoryManagement.Frontend.Pages.Product
{
    public partial class GetByIdProductAndDetails : ComponentBase
    {
        #region Inject
        [Inject] public CommunicationService? _communicationService { get; set; }
        [Inject] public NavigationManager? _navigationManager { get; set; }
        [Inject] public NotificationService? _notificationService { get; set; }
        [Inject] public ApiService? _apiService { get; set; }
        [Inject] public IAuthorizationService? _authorizationService { get; set; }
        [Inject] public AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        [Inject] public DialogService? _dialogService { get; set; }
        #endregion



        #region Fixed definitions
        private int SelectedProductId { get; set; }
        private int SelectedProductBarcode { get; set; }
        private AssignedProductApproveRejectDto? _assignedProductApproveRejectDto = new AssignedProductApproveRejectDto();
        private UpdateGetStoreProductCommandDto? _updateGetStoreDto = new UpdateGetStoreProductCommandDto();
        private readonly string _defaultImageBase64 = "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";
        #endregion

        #region Product
        private PaginatedResult<GetByIdProductAndDetailsDto>? _productDetailModelDto;
        #endregion

        #region User List
        private IEnumerable<KeycloakUsersModel>? _users;
        private KeycloakUsersModel? _selectedUser;
        #endregion

        #region Transfer
        private PaginatedResult<GetAllCompanyAndTransferOfficerDto>? _getAllCompanyAndTransferList { get; set; }
        private RadzenDataGrid<GetAllCompanyAndTransferOfficerDto>? _transferCenterGrid;
        private IList<GetAllCompanyAndTransferOfficerDto>? _selectedTransferCenter;
        private int _selectedTransferButton = 0;
        #endregion


        #region Report
        DxReportViewer? _reportViewer;
        XtraReport _report = XtraReport.FromFile(ApplicationConstants.ZimmetFormuReport, true);
        #endregion



        protected override async Task OnInitializedAsync()
        {
            SelectedProductId = _communicationService.GetSelectedProductId();
            _communicationService.OnProductIdSelected += SetSelectedProductId;
            if (SelectedProductId == null || SelectedProductId == 0)
            {
                _navigationManager?.NavigateTo("/product/product-and-assigned");
                return;
            }
            await ProductGetDetails();

            _selectedTransferCenter = _getAllCompanyAndTransferList?.data?.Take(1).ToList();

            _updateGetStoreDto.Id = SelectedProductId;


            SelectedProductBarcode = _communicationService.GetSelectedProductBarcode();
            _communicationService.OnProductBarcodeSelected += SetSelectedProductBarcode;
        }




        private async Task SetSelectedProductId(int selectedProduct)
        {
            SelectedProductId = selectedProduct;
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        private async Task SetSelectedProductBarcode(int selectedProduct)
        {
            SelectedProductBarcode = selectedProduct;
        }



        async Task ProductGetDetails()
        {
            try
            {
                _productDetailModelDto = await _apiService.GetAsync<PaginatedResult<GetByIdProductAndDetailsDto>>($"{ApiEndpointConstants.GetByIdProductAndDetailsQuery}?Id={SelectedProductId}");
            }
            catch (Exception ex)
            {
                _notificationService?.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"İstek gerçekleştirilemedi {ex.Message}", duration: 6000);
            }
        }






        async Task ProcessMenu(RadzenSplitButtonItem item)
        {
            if (item != null && item.Value != null)
            {
                if (item.Value == "assignedUser")
                {
                    await ShowUsersDialog();
                }
                else if (item.Value == "assignedUserForm")
                {
                    #region Report
                    _report =  XtraReport.FromFile(ApplicationConstants.ZimmetFormuReport, true);
                    _report.DataSource = _productDetailModelDto;
                    _report.DataMember = "data";
                    #endregion
                    await ShowEmbezzlementFormDialog();
                }
                else if (item.Value == "getStore")
                {
                    await ShowGetStoreDialog();
                }
                else if (item.Value == "productTransfer")
                {
                    await ProductTransferGetList();
                    await ShowProductTransferDialog();
                }
            }
        }




        #region Product Assigned User
        private async Task LoadDropDownData(LoadDataArgs args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args?.Filter))
                {
                    return;
                }
                _users = await _apiService.GetAsync<List<KeycloakUsersModel>>($"{ApiEndpointConstants.KeyCloakUserSearchEndpoint}?search={args.Filter}");
            }
            catch (Exception ex)
            {
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
                _dialogService?.Refresh();
            });
        }
        private void OnUserSelectionChanged(object selectedUser)
        {
            _selectedUser = (KeycloakUsersModel)selectedUser;
            InvokeAsync(() =>
            {
                StateHasChanged();
                _dialogService?.Refresh();
            });
        }

        async Task AssignedProductCreate()
        {
            if (_selectedUser == null)
            {
                return;
            }

            AssignedProductsQueryDto assignedProductDto = new AssignedProductsQueryDto
            {
                AssignedUserId = _selectedUser.Id,
                AssignedUserName = _selectedUser.Attributes?.sAMAccountName?.FirstOrDefault(),
                ProductId = _productDetailModelDto.data.FirstOrDefault().Id,
                AssignedUserPhoto = _selectedUser?.Attributes?.thumbnailPhoto?[0],
                FullName = $"{_selectedUser?.FirstName} {_selectedUser?.LastName}",
                Email = _selectedUser?.Email,
                Barcode = _productDetailModelDto?.data?.FirstOrDefault()?.Barcode,
                ProductName = _productDetailModelDto?.data?.FirstOrDefault()?.Name,
                Id = _productDetailModelDto?.data?.FirstOrDefault()?.AssignedProducts?.FirstOrDefault()?.Id ?? 90
            };


            var endpoint = _productDetailModelDto?.data?.FirstOrDefault()?.AssignedProducts?.FirstOrDefault()?.Id == null
                            ? ApiEndpointConstants.AssignedProductCreate
                            : ApiEndpointConstants.AssignedProductUpdate;

            var result = _productDetailModelDto?.data?.FirstOrDefault()?.AssignedProducts?.FirstOrDefault()?.Id == null
                            ? await _apiService.PostAsync(endpoint, assignedProductDto)
                            : await _apiService.PutAsync(endpoint, assignedProductDto);


            if (result.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, "Zimmet", $"{_selectedUser?.FirstName} {_selectedUser?.LastName} kullanıcısına {_productDetailModelDto?.data?.FirstOrDefault()?.Id} numaralı ürün zimmetlendi.", duration: 6000);
                _productDetailModelDto.data.FirstOrDefault().Status = "Zimmetlendi";


                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    _notificationService.Notify(NotificationSeverity.Success, "Zimmet", $"{_selectedUser?.FirstName} {_selectedUser?.LastName} kullanıcısına {_productDetailModelDto?.data?.FirstOrDefault()?.Id} numaralı ürün zimmetlendi.", duration: 6000);
                    _productDetailModelDto.data.FirstOrDefault().Status = "Zimmetlendi";
                    UpdateAssignedProductInfo(_selectedUser);
                }
                else
                {

                    var json = await result.Content.ReadAsStringAsync();

                    var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var response = JsonSerializer.Deserialize<AssignedProductsQueryDto>(json, jsonOptions);
                    if (_productDetailModelDto.data.FirstOrDefault().AssignedProducts == null)
                    {
                        _productDetailModelDto.data.FirstOrDefault().AssignedProducts = new List<AssignedProductsQueryDto>();
                    }
                    _productDetailModelDto.data.FirstOrDefault().AssignedProducts.Add(response);
                    UpdateAssignedProductInfo(_selectedUser);
                }

            }

            //showAcceptRejectButtons = false;
            _dialogService?.Close();
            _dialogService?.Dispose();
            StateHasChanged();
        }
        void UpdateAssignedProductInfo(KeycloakUsersModel selectedUser)
        {
            if (_productDetailModelDto?.data == null)
            {
                return;
            }
            string userFullName = $"{selectedUser?.FirstName} {selectedUser?.LastName} {selectedUser?.AssignedUserName} {selectedUser?.Attributes?.sAMAccountName?.FirstOrDefault()}";
            string userPhoto = !string.IsNullOrEmpty(selectedUser?.Attributes?.thumbnailPhoto?.FirstOrDefault()) ? selectedUser.Attributes.thumbnailPhoto[0] : _defaultImageBase64;
            foreach (var product in _productDetailModelDto.data)
            {
                if (product.AssignedProducts != null)
                {
                    foreach (var p in product.AssignedProducts)
                    {
                        p.AssignedUserName = selectedUser?.AssignedUserName;
                        p.FullName = userFullName;
                        p.AssignedUserPhoto = userPhoto;
                    }
                }
            }
        }
        private string GetUserPhoto()
        {
            foreach (var product in _productDetailModelDto?.data ?? Enumerable.Empty<GetByIdProductAndDetailsDto>())
            {
                if (product.AssignedProducts != null)
                {
                    foreach (var assignedProduct in product.AssignedProducts)
                    {
                        if (!string.IsNullOrEmpty(assignedProduct.AssignedUserPhoto))
                        {
                            return $"data:image/jpeg;base64,{assignedProduct.AssignedUserPhoto}";
                        }
                    }
                }
            }
            return $"data:image/jpeg;base64,{_defaultImageBase64}";
        }
        #endregion


        #region Show Get Store
        async Task ProductGetStore()
        {
            try
            {
                var result = await _apiService.PutAsync(ApiEndpointConstants.GetStoreProduct, _updateGetStoreDto);
                if (result.IsSuccessStatusCode)
                {
                    await UpdateSelectedProductStoreInfo();
                    await UpdateAssignedProductsStoreInfo();
                }

                await InvokeAsync(() =>
                {
                    _dialogService?.Close();
                    StateHasChanged();
                });
            }
            catch (Exception)
            {
            }
        }

        async Task UpdateSelectedProductStoreInfo()
        {
            var firstProduct = _productDetailModelDto?.data?.FirstOrDefault();
            if (firstProduct != null)
            {
                var firstAssignedProduct = firstProduct?.AssignedProducts?.FirstOrDefault();

                if (firstAssignedProduct != null)
                {
                    firstAssignedProduct.AssignedUserId = null;
                    //firstAssignedProduct.Id = 0;
                    firstAssignedProduct.AssignedUserName = null;
                    firstAssignedProduct.FullName = null;
                }

                firstProduct.Status = "Depoda";

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }


        async Task UpdateAssignedProductsStoreInfo()
        {
            if (_productDetailModelDto?.data == null)
            {
                return;
            }
            foreach (var product in _productDetailModelDto.data)
            {
                if (product.AssignedProducts == null)
                {
                    continue;
                }
                foreach (var p in product.AssignedProducts)
                {
                    p.FullName = null;
                    p.AssignedUserName = null;
                    p.AssignedUserPhoto = _defaultImageBase64;
                }
            }
        }
        #endregion


        #region AssignedProductApproveReject
        async Task AssignedProductApproveReject(string status)
        {
            _assignedProductApproveRejectDto.ApprovalStatus = status;
            _assignedProductApproveRejectDto.ProductId = SelectedProductId;
            _assignedProductApproveRejectDto.AssignedProductId = _productDetailModelDto?.data?.FirstOrDefault()?.AssignedProducts?.FirstOrDefault()?.Id;

            try
            {
                var result = await _apiService.PutAsync(ApiEndpointConstants.AssignedProductApproveReject, _assignedProductApproveRejectDto);
                if (result.IsSuccessStatusCode)
                {
                    var severity = status == "Accepted" ? NotificationSeverity.Success : NotificationSeverity.Error;
                    var message = status == "Accepted" ? "onayladınız" : "reddettiniz";
                    var fullName = _productDetailModelDto?.data?.FirstOrDefault()?.AssignedProducts?.FirstOrDefault()?.FullName;
                    var barcode = _productDetailModelDto?.data?.FirstOrDefault()?.Barcode;

                    if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(barcode))
                    {
                        _notificationService?.Notify(severity, status, $"Sayın {fullName}, {barcode} numaralı ürünü {message}.", duration: 6000);
                    }
                }
            }
            catch (Exception)
            {
            }
            await InvokeAsync(() =>
            {
                _dialogService?.Close();
                StateHasChanged();
            });
        }
        #endregion


        #region Transfer
        async Task ProductTransferGetList()
        {
            try
            {
                _getAllCompanyAndTransferList = await _apiService.GetAsync<PaginatedResult<GetAllCompanyAndTransferOfficerDto>>($"{ApiEndpointConstants.TransferOfficierCrud}/GetAll");
            }
            catch (Exception)
            {
                // Hata yönetimi işlemleri
            }
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        async Task SetSelectProductTransfer(GetAllCompanyAndTransferOfficerDto args)
        {
            _selectedTransferCenter = new List<GetAllCompanyAndTransferOfficerDto> { args };

            await InvokeAsync(() =>
            {
                StateHasChanged();
                _dialogService?.Refresh();
            });
        }
        async Task ProductTransfer()
        {
            string senderUserName = ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername;
            string senderEmail = ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Email;
            var selectedTransferCenter = _selectedTransferCenter[0];
            ProductOperationsModel productTransfer = new ProductOperationsModel
            {
                Id = SelectedProductId,
                SenderUserName = senderUserName,
                SenderEmail = senderEmail,
                RecipientCompanyId = selectedTransferCenter.CompanyId,
                RecipientUserName = selectedTransferCenter.UserName,
                RecipientFullName = selectedTransferCenter.FullName,
                RecipientEmail = selectedTransferCenter.Email,
                RecipientCompanyName = selectedTransferCenter.CompanyName,
                TypeOfOperations = GenericConstantDefinitions.Transfer
            };
            try
            {
                var result = await _apiService.PutAsync(ApiEndpointConstants.ProductTransfer, productTransfer);
                if (result.IsSuccessStatusCode)
                {
                    string barcode = _productDetailModelDto?.data?.FirstOrDefault()?.Barcode;
                    _notificationService?.Notify(NotificationSeverity.Success, "Transfer", $"{barcode} barkodlu ürün transfer işlemi başladı.", duration: 6000);
                    _productDetailModelDto.data.FirstOrDefault().Status = "Transfer Aşamasında";
                }
            }
            catch (Exception)
            {
            }

            await InvokeAsync(() =>
            {
                _dialogService?.Close();
                StateHasChanged();
            });
        }
        async Task ProductTransfer(string transferValue)
        {
            string typeOfOperation = null;
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
            ProductOperationsModel productTransfer = new ProductOperationsModel
            {
                Id = SelectedProductId,
                TypeOfOperations = typeOfOperation
            };
            try
            {
                var result = await _apiService.PutAsync(ApiEndpointConstants.ProductTransfer, productTransfer);
                if (result.IsSuccessStatusCode)
                {
                    string barcode = _productDetailModelDto?.data?.FirstOrDefault()?.Barcode;
                    string status = GetStatusForTransferValue(transferValue);

                    _notificationService?.Notify(NotificationSeverity.Success, "Transfer", $"{barcode} barkodlu ürün için {transferValue} işlemi yapıldı", duration: 6000);
                    _productDetailModelDto.data.FirstOrDefault().Status = status;
                }
            }
            catch (Exception)
            {
            }

            await InvokeAsync(() =>
            {
                _dialogService?.Close();
                StateHasChanged();
            });
        }
        private string GetStatusForTransferValue(string transferValue)
        {
            switch (transferValue)
            {
                case GenericConstantDefinitions.Accepted:
                    return GenericConstantDefinitions.InStock;
                case GenericConstantDefinitions.Rejected:
                    return GenericConstantDescriptions.Descriptions[GenericConstantDefinitions.Rejected];
                case GenericConstantDefinitions.ReturnIt:
                    return GenericConstantDescriptions.Descriptions[GenericConstantDefinitions.ReturnIt];
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}

