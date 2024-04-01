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


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected RadzenBody body0;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected RadzenSidebar sidebar0;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        protected async Task SidebarToggle0Click(dynamic args)
        {
            await InvokeAsync(() => { sidebar0.Toggle(); });
            await InvokeAsync(() => { body0.Toggle(); });
        }





        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                await AuthorizationService.InitializeAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                await UpdateIsAuthorized();
            }
        }


        private bool isAuthorized;
        private async Task UpdateIsAuthorized()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            isAuthorized = authenticationState.User.Identity.IsAuthenticated;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
