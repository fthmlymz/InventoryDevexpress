using ApiManagement.Gateway;
using Gateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


#region Docker - Kestrel
//Development ekranında kapatılabilir Kestrel
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
                listenOptions.UseHttps(builder.Configuration.GetSection("ASPNETCORE_Kestrel:Certificates:Default:Path").Value,
                    builder.Configuration.GetSection("ASPNETCORE_Kestrel:Certificates:Default:Password").Value);
            });
        }
    }
});
#endregion




#region CORS Settings
var allowedResources = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedResources,
                      policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});
#endregion





#region Ocelot Settings
var environment = builder.Environment;
var ocelotConfigFileName = environment.IsDevelopment() ? "ocelot.development.json" : "ocelot.json";
var appsettingsConfigFileName = environment.IsDevelopment() ? "appsettings.development.json" : "appsettings.json";

builder.Configuration.AddJsonFile(ocelotConfigFileName, optional: false, reloadOnChange: true);
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(appsettingsConfigFileName, optional: false, reloadOnChange: true)
    .AddJsonFile(ocelotConfigFileName, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var ocelotHosts = configuration.GetSection("GatewaySettings:Hosts").GetChildren().ToDictionary(child => child.Key, child => child.Value);
var ocelotConfigSection = configuration.GetSection("GlobalConfiguration:Hosts");

foreach (var kvp in ocelotHosts)
{
    ocelotConfigSection[kvp.Key] = kvp.Value;
}

builder.Services.AddOcelot(builder.Configuration);
builder.Services.ConfigureDownstreamHostAndPortsPlaceholders(configuration);
#endregion








#region Keycloak - JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.Authority = builder.Configuration.GetSection("Keycloak:Authority").Value;
           options.Audience = builder.Configuration.GetSection("Keycloak:Audience").Value;
           options.RequireHttpsMetadata = false;
           options.UseSecurityTokenValidators = true; //sonradan eklendi
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true, //false
               ValidateAudience = true, //false
               ValidateLifetime = true, //false
               ValidateIssuerSigningKey = true, //false
               ValidIssuer = builder.Configuration.GetSection("Keycloak:Authority").Value,
               ValidAudience = builder.Configuration.GetSection("Keycloak:Audience").Value,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Keycloak:ClientSecret").Value))
           };
       });
IdentityModelEventSource.ShowPII = true;
#endregion







var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseCors(allowedResources);
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseFormContentMiddleware();
await app.UseOcelot();

app.Run();
