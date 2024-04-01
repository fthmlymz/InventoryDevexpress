using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});



#region Docker - Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var httpPort = builder.Configuration["ASPNETCORE_HTTP_PORTS"];
    var httpsPort = builder.Configuration["ASPNETCORE_HTTPS_PORTS"];


    if (builder.Environment.IsDevelopment() || builder.Environment.IsProduction())
    {
        if (!string.IsNullOrEmpty(httpPort))
        {
            serverOptions.ListenAnyIP(Convert.ToInt32(httpPort));
        }

        if (!string.IsNullOrEmpty(httpsPort))
        {
            serverOptions.ListenAnyIP(Convert.ToInt32(httpsPort), listenOptions =>
            {
#pragma warning disable CS8604 // Possible null reference argument.
                listenOptions.UseHttps(builder.Configuration.GetSection("ASPNETCORE_Kestrel:Certificates:Default:Path").Value,
                    builder.Configuration.GetSection("ASPNETCORE_Kestrel:Certificates:Default:Password").Value);
#pragma warning restore CS8604 // Possible null reference argument.
            });
        }
    }
});
#endregion







builder.Services.AddHttpClient<HttpClient>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());
builder.Services.AddRadzenComponents();

//// Sayfalar arasi veri iletimi
builder.Services.AddSingleton<CommunicationService>();


//// Permissions
builder.Services.AddScoped<IAuthorizationService, PermissionsService>();


#region Keycloak
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, KeycloakAuthenticationStateProvider>();
#endregion

//// ApiService-NotificationService
builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<NotificationService>();

ApiEndpointConstants.Load(builder.Configuration);


#region Reporting
builder.WebHost.UseStaticWebAssets(); //Radzen
builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressServerSideBlazorReportViewer();
#endregion








var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    
}

app.UseHttpsRedirection(); // HTTPS - SSL
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
