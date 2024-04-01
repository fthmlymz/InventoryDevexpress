using DevExpress.Blazor;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Keycloak;
using InventoryManagement.Frontend.Services;
using Microsoft.AspNetCore.Components;

namespace InventoryManagement.Frontend.Pages.Keycloak
{
    public partial class UserPage : ComponentBase
    {
        [Inject] public ApiService? ApiService { get; set; }
        [Inject] public CommunicationService? CommunicationService { get; set; }

        private IEnumerable<KeycloakUsersDto>? _users;
        IGrid ? GridUsers { get; set; }

        
        private async Task GetUsers(string args)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                if (string.IsNullOrWhiteSpace(args))
                {
                    return;
                }
                _users = await ApiService!.GetAsync<List<KeycloakUsersDto>>($"{ApiEndpointConstants.KeycloakUserSearchEndpoint}?search={args}");
                StateHasChanged();
            }
            catch (Exception ex)
            {
            }
#pragma warning restore CS0168 // Variable is declared but never used

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }


        async Task OnSelectedDataItemChanged(object newSelection)
        {
            await CommunicationService!.SendSelectedUser((KeycloakUsersDto)newSelection);
        }
    }
}
