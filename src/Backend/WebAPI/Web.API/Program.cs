using Application.Common.Exceptions;
using Application.Common.Filters;
using Application.Extensions;
using Application.Features.Products.Commands.ProductOperations;
using Application.Keycloak;
using Application.Workflows.Product;
using DotNetCore.CAP.Messages;
using IdentityModel.Client;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Context;
using Persistence.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using WorkflowCore.Interface;

var builder = WebApplication.CreateBuilder(args);





builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new ValidateFilterAttribute());
    opt.Filters.Add(typeof(ValidateJsonModelFilter));
})
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.IgnoreReadOnlyFields = true;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull; //db'de null olan deðerleri getirme
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





#region Dependency Injection
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddPersistenceLayer(builder.Configuration);
#endregion



#region Eventbus DotnetCore.CAP
builder.Services.AddCap(x =>
{
    x.UseEntityFramework<DotnetCapDbContext>();
    x.UseSqlServer(builder.Configuration.GetConnectionString("CapLogSqlServerConnection"));
    x.UseRabbitMQ(options =>
    {
        options.ExchangeName = "InventoryManagement.API";
        options.BasicQosOptions = new DotNetCore.CAP.RabbitMQOptions.BasicQos(3);
        options.ConnectionFactoryOptions = opt =>
        {
            opt.HostName = builder.Configuration.GetSection("RabbitMQ:Host").Value;
            opt.UserName = builder.Configuration.GetSection("RabbitMQ:Username").Value;
            opt.Password = builder.Configuration.GetSection("RabbitMQ:Password").Value;
            opt.Port = Convert.ToInt32(builder.Configuration.GetSection("RabbitMQ:Port").Value);
            opt.CreateConnection();
        };
    });
    x.UseDashboard(opt => opt.PathMatch = "/cap-dashboard");
    x.FailedRetryCount = 5;
    x.UseDispatchingPerGroup = true;
    x.FailedThresholdCallback = failed =>
    {
        var logger = failed.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                        requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
    };
    x.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});
#endregion


#region CORS Settings
var allowedResources = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedResources, policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});
#endregion


#region Keycloak JWT
var keycloakSettings = builder.Configuration.GetSection("Keycloak").Get<KeycloakSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.Authority = keycloakSettings?.Authority;
               options.Audience = keycloakSettings?.Audience;
               options.RequireHttpsMetadata = false;
               options.UseSecurityTokenValidators = true;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,
                   RequireSignedTokens = false,
                   ValidIssuer = keycloakSettings?.Authority, //test eklendi
                   ValidAudience = keycloakSettings?.Audience, //test eklendi
                   SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                   {
                       var jwt = new JwtSecurityToken(token);

                       return jwt;
                   },
                   ValidateLifetime = true,
                   RequireExpirationTime = true,
                   ClockSkew = TimeSpan.Zero,
               };
               options.Events = new JwtBearerEvents
               {
                   OnTokenValidated = context =>
                   {
                       return Task.CompletedTask;
                   },
               };
           });
builder.Services.AddTransient<IClaimsTransformation>(_ => new KeycloakRolesClaimsTransformation("role", keycloakSettings.Audience));
builder.Services.AddAuthorization(options =>
{
    #region Company
    options.AddPolicy("CompanyReadRole", builder => { builder.AddRequirements(new RptRequirement("res:company", "scopes:read")); });
    options.AddPolicy("CompanyCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:company", "scopes:create")); });
    options.AddPolicy("CompanyUpdateRole", builder => { builder.AddRequirements(new RptRequirement("res:company", "scopes:update")); });
    options.AddPolicy("CompanyDeleteRole", builder => { builder.AddRequirements(new RptRequirement("res:company", "scopes:delete")); });
    #endregion

    #region Category
    options.AddPolicy("CategoryReadRole", builder => { builder.AddRequirements(new RptRequirement("res:category", "scopes:read")); });
    options.AddPolicy("CategoryCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:category", "scopes:create")); });
    options.AddPolicy("CategoryUpdateRole", builder => { builder.AddRequirements(new RptRequirement("res:category", "scopes:update")); });
    options.AddPolicy("CategoryDeleteRole", builder => { builder.AddRequirements(new RptRequirement("res:category", "scopes:delete")); });
    #endregion

    #region CategorySub
    options.AddPolicy("CategorySubReadRole", builder => { builder.AddRequirements(new RptRequirement("res:categorysub", "scopes:read")); });
    options.AddPolicy("CategorySubCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:categorysub", "scopes:create")); });
    options.AddPolicy("CategorySubUpdateRole", builder => { builder.AddRequirements(new RptRequirement("res:categorysub", "scopes:update")); });
    options.AddPolicy("CategorySubDeleteRole", builder => { builder.AddRequirements(new RptRequirement("res:categorysub", "scopes:delete")); });
    #endregion

    #region Brand Permissions
    options.AddPolicy("BrandReadRole", builder => { builder.AddRequirements(new RptRequirement("res:brand", "scopes:read")); });
    options.AddPolicy("BrandCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:brand", "scopes:create")); });
    options.AddPolicy("BrandUpdateRole", builder => { builder.AddRequirements(new RptRequirement("res:brand", "scopes:update")); });
    options.AddPolicy("BrandDeleteRole", builder => { builder.AddRequirements(new RptRequirement("res:brand", "scopes:delete")); });
    #endregion

    #region Model Permissions
    //options.AddPolicy("ModelReadRole", builder => { builder.AddRequirements(new RptRequirement("res:model", "scopes:read")); });
    options.AddPolicy("ModelCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:model", "scopes:create")); });
    options.AddPolicy("ModelUpdateRole", builder => { builder.AddRequirements(new RptRequirement("res:model", "scopes:update")); });
    options.AddPolicy("ModelDeleteRole", builder => { builder.AddRequirements(new RptRequirement("res:model", "scopes:delete")); });
    #endregion

    #region Inventory
    options.AddPolicy("ProductReadRole", builder => { builder.AddRequirements(new RptRequirement("res:product", "scopes:read")); });
    options.AddPolicy("ProductCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:product", "scopes:create")); });
    options.AddPolicy("ProductUpdateRole", builder => { builder.AddRequirements(new RptRequirement("res:product", "scopes:update")); });
    options.AddPolicy("ProductDeleteRole", builder => { builder.AddRequirements(new RptRequirement("res:product", "scopes:delete")); });
    #endregion

    #region AssignedProduct
    options.AddPolicy("AssignedProductReadRole", builder => { builder.AddRequirements(new RptRequirement("res:assignedproduct", "scopes:read")); });
    options.AddPolicy("AssignedProductCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:assignedproduct", "scopes:create")); });
    options.AddPolicy("AssignedProductUpdateRole", builder => { builder.AddRequirements(new RptRequirement("res:assignedproduct", "scopes:update")); });
    #endregion

    #region TransferOfficier
    options.AddPolicy("TransferOfficierReadRole", builder => { builder.AddRequirements(new RptRequirement("res:transferofficier", "scopes:read")); });
    options.AddPolicy("TransferOfficierCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:transferofficier", "scopes:create")); });
    options.AddPolicy("TransferOfficierUpdateRole", builder => { builder.AddRequirements(new RptRequirement("res:transferofficier", "scopes:update")); });
    options.AddPolicy("TransferOfficierDeleteRole", builder => { builder.AddRequirements(new RptRequirement("res:transferofficier", "scopes:delete")); });
    #endregion

    #region Report
    options.AddPolicy("ReportReadRole", builder => { builder.AddRequirements(new RptRequirement("res:report", "scopes:read")); });
    options.AddPolicy("ReportCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:report", "scopes:create")); });
    #endregion


    #region TransferOfficier
    options.AddPolicy("FileTransferCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:filetransfer", "scopes:create")); });
    #endregion


    #region File Transfer
    options.AddPolicy("FileTransferReadRole", builder => { builder.AddRequirements(new RptRequirement("res:filetransfer", "scopes:read")); });
    options.AddPolicy("FileTransferCreateRole", builder => { builder.AddRequirements(new RptRequirement("res:filetransfer", "scopes:create")); });
    options.AddPolicy("FileTransferDeleteRole", builder => { builder.AddRequirements(new RptRequirement("res:filetransfer", "scopes:delete")); });
    #endregion
});

builder.Services.AddHttpClient<KeycloakService>(client =>
{
    client.BaseAddress = new Uri(keycloakSettings.KeycloakResourceUrl);
});
builder.Services.AddHttpClient<IdentityModel.Client.TokenClient>();
builder.Services.AddSingleton(new ClientCredentialsTokenRequest
{
    Address = keycloakSettings?.ClientCredentialsTokenAddress
});
builder.Services.AddScoped<IAuthorizationHandler, RptRequirementHandler>();
#endregion


#region Workflow
builder.Services.AddWorkflow(cfg =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection");
    cfg.UseSqlServer(connectionString, true, true);
});
#endregion



#region Kestrel docker-compose.yml
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





var app = builder.Build();



#region Auto migrate
using (var scope = app.Services.CreateScope())
{
    var dotnetCapContext = scope.ServiceProvider.GetRequiredService<DotnetCapDbContext>();
    var apiContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


    dotnetCapContext.Database.Migrate();
    apiContext.Database.Migrate();
}
#endregion


#region Workflow Definitions
var host = app.Services.GetRequiredService<IWorkflowHost>();
host.RegisterWorkflow<ProductTransferWorkflow, UpdateProductOperationsCommand>();
host.Start();
#endregion



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(allowedResources);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<UseCustomExceptionHandler>();
app.Run();
