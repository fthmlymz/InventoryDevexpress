using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using InventoryManagement.Frontend.Services.Authorization;

namespace InventoryManagement.Frontend.Shared
{
    public partial class MainLayoutComponent : LayoutComponentBase
    {
        [Inject]  DialogService ? DialogService { get; set; }
        [Inject]  NotificationService ? NotificationService { get; set; }
        [Inject] NavigationManager ? _navigationManager { get; set; }
        [Inject] public AuthenticationStateProvider ? authenticationStateProvider { get; set; }
        [Inject] public IAuthorizationService? AuthorizationService { get; set; }


        protected RadzenBody body0;
        protected RadzenSidebar sidebar0;


        protected async Task SidebarToggle0Click(dynamic args)
        {
            await InvokeAsync(() => { sidebar0.Toggle(); });
            await InvokeAsync(() => { body0.Toggle(); });
        }





        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await AuthorizationService.InitializeAsync();
                await UpdateIsAuthorized();
            }
        }


        private bool isAuthorized;
        private async Task UpdateIsAuthorized()
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            isAuthorized = authenticationState.User.Identity.IsAuthenticated;
            StateHasChanged();

            if (!isAuthorized)
            {
                _navigationManager?.NavigateTo("/login", forceLoad: true);
            }
            else
            {
                _navigationManager?.NavigateTo("/");
            }
        }
    }
}
