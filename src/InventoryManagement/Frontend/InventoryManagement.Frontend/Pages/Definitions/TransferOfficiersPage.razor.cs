using DevExpress.Blazor;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Company;
using InventoryManagement.Frontend.DTOs.Keycloak;
using InventoryManagement.Frontend.DTOs.TransferOfficier;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace InventoryManagement.Frontend.Pages.Definitions
{
    public partial class TransferOfficiersPage : ComponentBase
    {
        #region Inject
        [Inject] ApiService? ApiService { get; set; }
        [Inject] IAuthorizationService? AuthorizationService { get; set; }
        [Inject] NotificationService? NotificationService { get; set; }
        [Inject] DialogService? DialogService { get; set; }
        [Inject] public CommunicationService? CommunicationService { get; set; }
        #endregion

        private bool ShowUsersVisible { get; set; } = false;
        public KeycloakUsersDto? SelectedUser { get; set; }
        IGrid? GridTransferOfficier { get; set; }

        private PaginatedResult<TransferOfficierDto>? transferOfficiers { get; set; }
        private PaginatedResult<CompanyDto>? companyList;

        protected override async void OnInitialized()
        {
            await Task.WhenAll(GetTransferOfficiers(), GetCompanyList());
        }

        async Task GetTransferOfficiers()
        {
            transferOfficiers = await ApiService!.GetAsync<PaginatedResult<TransferOfficierDto>>($"{ApiEndpointConstants.GetTransferOfficierGetAll}");
            StateHasChanged();
        }

        async Task GetCompanyList()
        {
            companyList = await ApiService!.GetAsync<PaginatedResult<CompanyDto>>($"{ApiEndpointConstants.AllCompanyList}");
            StateHasChanged();
        }


        #region EditModel
        void GridTransferOfficier_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var res = (TransferOfficierDto)e.EditModel;
                res.CompanyId = 0;
                SelectedUser = null;
            }
        }
        async Task GridTransferOfficier_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                var newTransferOfficier = (TransferOfficierDto)e.EditModel;

                newTransferOfficier.FullName = SelectedUser?.FirstName + " " + SelectedUser?.LastName;
                newTransferOfficier.Email = SelectedUser?.Email;
                newTransferOfficier.UserName = SelectedUser?.Username;

                if (newTransferOfficier.CompanyId == 0 || SelectedUser == null) return;

                var response = await ApiService!.PostAsync(ApiEndpointConstants.PostTransferOfficier, newTransferOfficier);

                if (response.IsSuccessStatusCode)
                {
                    var insertedTransferOfficier = await response.Content.ReadFromJsonAsync<TransferOfficierDto>();
                    if (insertedTransferOfficier != null)
                    {
                        transferOfficiers?.data?.Add(insertedTransferOfficier);
                        transferOfficiers!.totalCount++;
                        StateHasChanged();
                    }
                }
                StateHasChanged();
            }
            else
            {
                var updatedTransferOfficier = (TransferOfficierDto?)e.EditModel;
                updatedTransferOfficier!.FullName = SelectedUser?.FirstName + " " + SelectedUser?.LastName;
                updatedTransferOfficier!.Email = SelectedUser?.Email;
                updatedTransferOfficier!.UserName = SelectedUser?.Username;

                var response = await ApiService!.PutAsync($"{ApiEndpointConstants.PutTransferOfficier}", updatedTransferOfficier);
                if (response.IsSuccessStatusCode)
                {
                    var index = transferOfficiers!.data!.FindIndex(c => c.Id == updatedTransferOfficier?.Id);
                    if (index != -1)
                    {
                        transferOfficiers.data[index] = updatedTransferOfficier;
                        StateHasChanged();
                    }
                }
            }
        }

        async Task DeleteTransferOfficier(TransferOfficierDto data)
        {
            bool? confirmed = await DialogService!.Confirm($"{data.FullName} isimli personel şirket altından kaldırılacak onaylıyor musunuz?\n" +
                                         "Silme Onayı");
            if (confirmed == true)
            {
                var response = await ApiService!.DeleteAsync(ApiEndpointConstants.DeleteTransferOfficier, data.Id);
                if (response.IsSuccessStatusCode)
                {
                    transferOfficiers?.data?.RemoveAll(item => item.Id == data.Id);

                    StateHasChanged();
                    GridTransferOfficier?.Reload();
                    NotificationService?.Notify(NotificationSeverity.Success, "Başarılı", $"Silme isteği gerçekleşti.");
                }
            }
        }
        #endregion


        void UserSelect()
        {
            SelectedUser = CommunicationService?.GetSelectedUser();
        }
    }
}
