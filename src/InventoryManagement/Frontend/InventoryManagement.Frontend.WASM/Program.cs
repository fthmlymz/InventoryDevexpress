using Blazored.SessionStorage;
using InventoryManagement.Frontend;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Sayfalar arasi veri iletimi
builder.Services.AddSingleton<CommunicationService>();


//Permissions
builder.Services.AddScoped<IAuthorizationService, PermissionsService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
var configuration = builder.Configuration;
ApiEndpointConstants.Load(configuration);





#region Keycloak
builder.Services.AddAuthorizationCore(); // Authorizationn icin client uygulamasinda olacak
builder.Services.AddScoped<AuthenticationStateProvider, KeycloakAuthenticationStateProvider>();
#endregion

builder.Services.AddBlazoredSessionStorage();


// HttpClient'i yapilandir
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


// ApiService'i kaydet
builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<DialogService>();
builder.Services.AddSingleton<ContextMenuService>();


#region Devexpress
//builder.Services.AddDevExpressBlazor();
//builder.Services.AddDevExpressWebAssemblyBlazorReportViewer();
#endregion


await builder.Build().RunAsync();
